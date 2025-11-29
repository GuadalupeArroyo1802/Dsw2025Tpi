namespace Dsw2025Tpi.Application.Dtos
{
    public record OrderModel
    {
        // Requests
        public record OrderRequest(
            string ShippingAddress,
            string BillingAddress,
            Guid CustomerId,
            List<OrderItemModel> OrderItems
        );

        public record OrderItemModel(
            Guid ProductId,
            int Quantity
        );

        // Responses
        public record OrderResponse(
            Guid Id,
            string? ShippingAddress,
            string? BillingAddress,
            Guid CustomerId,
            string CustomerName,              //  nombre del cliente
            string Email,                     //  email del cliente
            DateTime Date,
            decimal TotalAmount,
            List<OrderItemResponse> OrderItems,
            string Status
        );

        public record OrderItemResponse(
            Guid ProductId,
            string Name,
            string Description,
            decimal UnitPrice,
            int Quantity,
            decimal Subtotal
        );

        public record OrderQueryParameters(
            string? Status,
            Guid? CustomerId,
            int PageNumber = 1,
            int PageSize = 10
        );

        public record UpdateOrderStatusRequest(
            string NewStatus
        );
    }
}
