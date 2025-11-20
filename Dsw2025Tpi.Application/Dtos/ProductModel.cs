namespace Dsw2025Tpi.Application.Dtos
// Definicion de los modelos de datos para productos
// por cada peticion, obtiene una respuesta con los datos del producto
{
    public record ProductModel
    {
        public record ProductRequest(
            string Sku,
            string InternalCode,
            string Name,
            string Description,
            decimal CurrentUnitPrice,
            int StockQuantity
            );

        public record ProductResponse(
            Guid Id,
            string Sku,
            string Name,
            decimal Price,
            string InternalCode,
            string Description,
            int Stock,
            bool isActive
            );

        public record ProductResponseUpdate(
            Guid Id,
            string Sku,
            string Name,
            decimal Price,
            string InternalCode,
            string Description,
            int Stock,
            bool IsActive
            );
        public record ProductResponseID(
            Guid Id
            );
        public record ResponsePagination(
            List<ProductResponse> ProductItems,
            int Total
            );
        public record FilterProduct(
            string? Status,
            string? Search,
            int? PageNumber,
            int? PageSize
            );
    }
}