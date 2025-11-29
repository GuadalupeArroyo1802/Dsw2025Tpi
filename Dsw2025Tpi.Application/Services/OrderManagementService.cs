using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using static Dsw2025Tpi.Application.Dtos.OrderModel;

namespace Dsw2025Tpi.Application.Services
{
    public class OrderManagement : IOrderManagement
    {
        private readonly IRepository _repository;

        public OrderManagement(IRepository repostory)
        {
            _repository = repostory;
        }

        public async Task<OrderResponse> AddOrder(OrderRequest request)
        {
            if (request.CustomerId == Guid.Empty ||
                string.IsNullOrWhiteSpace(request.ShippingAddress) ||
                string.IsNullOrWhiteSpace(request.BillingAddress))
            {
                throw new ArgumentException("Ingrese dirección de envio y/o facturación");
            }

            if (request == null || request.OrderItems == null || !request.OrderItems.Any())
                throw new ArgumentException("La orden debe tener al menos un producto.");

            var customer = await _repository.GetById<Customer>(request.CustomerId);
            if (customer == null)
                throw new EntityNotFoundException($"Cliente no encontrado.");

            // Validaciones de productos
            foreach (var item in request.OrderItems)
            {
                if (item.Quantity <= 0)
                    throw new ArgumentException($"La cantidad del producto con ID {item.ProductId} debe ser mayor a cero.");

                var product = await _repository.GetById<Product>(item.ProductId);
                if (product == null)
                    throw new EntityNotFoundException($"Producto con ID {item.ProductId} no encontrado.");
                if (product.IsActive == false)
                    throw new ArgumentException("producto no disponible, campo IsActive false");
                if (product.StockQuantity < item.Quantity)
                    throw new ArgumentException($"No hay suficiente stock para el producto {product.Name}.");
                else
                {
                    product.RestarStock(item.Quantity);
                    await _repository.Update(product);
                }
            }

           
            var orderItems = new List<OrderItem>();
            foreach (var item in request.OrderItems)
            {
                var product = await _repository.GetById<Product>(item.ProductId);
                var orderItem = new OrderItem(product.Id, product, item.Quantity, product.CurrentUnitPrice);
                orderItems.Add(orderItem);
            }

            var order = new Order(
                customerId: customer.Id,
                shippingAddress: request.ShippingAddress,
                billingAddress: request.BillingAddress,
                orderItems: orderItems,
                createdAt: DateTime.UtcNow,
                status: OrderStatus.Pending
            );

            var added = await _repository.Add(order);

            
            return new OrderResponse(
                Id: added.Id,
                ShippingAddress: added.ShippingAddress,
                BillingAddress: added.BillingAddress,
                CustomerId: added.CustomerId,
                CustomerName: customer.Name ?? string.Empty,
                Email: customer.Email ?? string.Empty,
                Date: added.CreatedAt,
                TotalAmount: added.TotalAmount,
                OrderItems: added.OrderItems.Select(oi => new OrderItemResponse(
                    ProductId: oi.ProductId,
                    Name: oi.Product?.Name ?? "",
                    Description: oi.Product?.Description ?? "",
                    UnitPrice: oi.UnitPrice,
                    Quantity: oi.Quantity,
                    Subtotal: oi.Subtotal
                )).ToList(),
                Status: added.Status.ToString()
            );
        }

        // Obtener todas las órdenes
        public async Task<IEnumerable<OrderResponse>> GetOrders(string? status, Guid? customerId, int pageNumber = 1, int pageSize = 10)
        {
            var allOrders = await _repository.GetAll<Order>(
                nameof(Order.Customer),
                nameof(Order.OrderItems),
                $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"
            );

            if (allOrders == null) return Enumerable.Empty<OrderResponse>();

            var query = allOrders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            var pagedOrders = query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return pagedOrders.Select(o => new OrderResponse(
                Id: o.Id,
                ShippingAddress: o.ShippingAddress,
                BillingAddress: o.BillingAddress,
                CustomerId: o.CustomerId,
                CustomerName: o.Customer?.Name ?? string.Empty,   
                Email: o.Customer?.Email ?? string.Empty,         
                Date: o.CreatedAt,
                TotalAmount: o.TotalAmount,
                OrderItems: o.OrderItems.Select(i => new OrderItemResponse(
                    ProductId: i.ProductId,
                    Name: i.Product?.Name ?? "",
                    Description: i.Product?.Description ?? "",
                    UnitPrice: i.UnitPrice,
                    Quantity: i.Quantity,
                    Subtotal: i.Subtotal
                )).ToList(),
                Status: o.Status.ToString()
            ));
        }

        // Obtener una orden por ID
        public async Task<OrderResponse?> GetOrderById(Guid id)
        {
            var order = await _repository.GetById<Order>(
                id,
                nameof(Order.Customer),                    
                nameof(Order.OrderItems),
                $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"
            );

            if (order == null) return null;

            return new OrderResponse(
                Id: order.Id,
                ShippingAddress: order.ShippingAddress,
                BillingAddress: order.BillingAddress,
                CustomerId: order.CustomerId,
                CustomerName: order.Customer?.Name ?? string.Empty,
                Email: order.Customer?.Email ?? string.Empty,
                Date: order.CreatedAt,
                TotalAmount: order.TotalAmount,
                OrderItems: order.OrderItems.Select(i => new OrderItemResponse(
                    ProductId: i.ProductId,
                    Name: i.Product?.Name ?? "",
                    Description: i.Product?.Description ?? "",
                    UnitPrice: i.UnitPrice,
                    Quantity: i.Quantity,
                    Subtotal: i.Subtotal
                )).ToList(),
                Status: order.Status.ToString()
            );
        }

        // Actualizar el estado de una orden
        public async Task<OrderResponse?> UpdateOrderStatus(Guid orderId, string newStatus)
        {
            var order = await _repository.GetById<Order>(
                orderId,
                nameof(Order.Customer),                    
                nameof(Order.OrderItems),
                $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"
            );

            if (order == null)
                return null;

            if (!Enum.TryParse<OrderStatus>(newStatus, ignoreCase: true, out var parsedStatus))
                throw new ArgumentException("Estado inválido.");

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                throw new ArgumentException("No se puede modificar una orden entregada o cancelada.");

            order.ChangeStatus(parsedStatus);
            await _repository.Update(order);

            return new OrderResponse(
                Id: order.Id,
                ShippingAddress: order.ShippingAddress,
                BillingAddress: order.BillingAddress,
                CustomerId: order.CustomerId,
                CustomerName: order.Customer?.Name ?? string.Empty,
                Email: order.Customer?.Email ?? string.Empty,
                Date: order.CreatedAt,
                TotalAmount: order.TotalAmount,
                OrderItems: order.OrderItems.Select(i => new OrderItemResponse(
                    ProductId: i.ProductId,
                    Name: i.Product?.Name ?? "",
                    Description: i.Product?.Description ?? "",
                    UnitPrice: i.UnitPrice,
                    Quantity: i.Quantity,
                    Subtotal: i.Subtotal
                )).ToList(),
                Status: order.Status.ToString()
            );
        }
    }
}
