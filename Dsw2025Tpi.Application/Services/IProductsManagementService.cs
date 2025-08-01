using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Services
{
    public interface IProductsManagementService
    {
        Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request);
        Task<bool> DisableProductAsync(Guid id);
        Task<ProductModel.ProductResponseUpdate>? GetProductById(Guid id);
        Task<List<ProductModel.ProductResponseUpdate>?> GetProducts();
        Task<ProductModel.ProductResponseUpdate> UpdateAsync(ProductModel.ProductRequest request, Guid id);
    }
}