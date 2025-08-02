using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Services
{
    public interface IOrderManagement
    {
        Task<OrderModel.OrderResponse> AddOrder(OrderModel.OrderRequest request);
        Task<IEnumerable<OrderModel.OrderResponse>> GetOrders(string? status, Guid? customerId, int pageNumber = 1, int pageSize = 10);
        Task<OrderModel.OrderResponse?> GetOrderById(Guid id);
        Task<OrderModel.OrderResponse?> UpdateOrderStatus(Guid orderId, string newStatus);
    }
}