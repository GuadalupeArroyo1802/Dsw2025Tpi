using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using static Dsw2025Tpi.Application.Dtos.OrderModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dsw2025Tpi.Application.Services
{
    public class OrderManagement : IOrderManagement
    {
        private readonly IRepository _repository;
        public OrderManagement(IRepository repostory)
        {
            _repository = repostory;
        }

        public async Task<OrderModel.OrderResponse> AddOrder(OrderModel.OrderRequest request)
        {
            if (request.CustomerId == Guid.Empty ||
                string.IsNullOrWhiteSpace(request.ShippingAddress) ||
                string.IsNullOrWhiteSpace(request.BillingAddress))

            {
                throw new ArgumentException("Ingrese dirección de envio y/o facturación");
            }

            // Validar que la lista de OrderItems no esté vacía
            if (request == null || request.OrderItems == null || !request.OrderItems.Any())
                throw new ArgumentException("La orden debe tener al menos un producto.");

            // Validar existencia del cliente
            var customer = await _repository.GetById<Customer>(request.CustomerId);
            if (customer == null)
                throw new EntityNotFoundException($"Cliente no encontrado.");

            // validaciones
            foreach (var item in request.OrderItems)
            {
                var product = await _repository.GetById<Product>(item.ProductId);
                if (product.IsActive == false)
                    throw new ArgumentException("producto no dispnible, campo IsActive false");
                if (item.CurrentUnitPrice != product.CurrentUnitPrice)
                    throw new ArgumentException("Precio de producto no coincidente");
                if (product == null)
                    throw new EntityNotFoundException($"Producto con ID {item.ProductId} no encontrado.");
                if (item.Description != product.Description || item.Name != product.Name)
                    throw new ArgumentException("Datos de descripcion o nombre no coincidentes");
                if (product.StockCuantity < item.Quantity)
                    throw new ArgumentException($"No hay suficiente stock para el producto {product.Name}.");
                if (item.Quantity <= 0)
                    throw new ArgumentException($"La cantidad del producto {item.Name} debe ser mayor a 0.");
                if (item.CurrentUnitPrice <= 0)
                    throw new ArgumentException($"El precio del producto {item.Name} debe ser mayor a 0.");
                else
                {
                    product.RestarStock(item.Quantity); // Restar la cantidad del producto del stock
                    await _repository.Update(product);
                }

            }



            var orderItems = new List<OrderItem>();
            //Crear los items de la orden
            foreach (var item in request.OrderItems)
            {
                var product = await _repository.GetById<Product>(item.ProductId);
                var orderItem = new OrderItem(product.Id, product, item.Quantity, item.CurrentUnitPrice);
                orderItems.Add(orderItem);
            }
            // Crear la orden
            var order = new Order(customer.Id, request.ShippingAddress, request.BillingAddress,
                orderItems, DateTime.UtcNow, OrderStatus.Pending);

            var added = await _repository.Add(order);

            return new OrderModel.OrderResponse(
                added.Id,
                added.CustomerId,
                added.ShippingAddress,
                added.BillingAddress,
                added.CreatedAt,
                added.TotalAmount,
                added.OrderItems.Select(oi => new OrderModel.OrderItemResponse(
                    oi.ProductId,
                    oi.Product?.Name ?? "",
                    oi.Product?.Description ?? "",
                    oi.UnitPrice,
                    oi.Quantity,
                    oi.Subtotal)).ToList(),
                added.Status.ToString());
        }
    }
}