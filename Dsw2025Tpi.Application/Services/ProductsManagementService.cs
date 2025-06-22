using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
  
    public class ProductsManagementService
        {
            private readonly IRepository _repository;

            public ProductsManagementService(IRepository repository)
            {
                _repository = repository;
            }

            public async Task<ProductModel.Response?> GetProductById(Guid id)
            {
                var product = await _repository.GetById<Product>(id);
                return product is not null && product.IsActive
                    ? new ProductModel.Response(
                        product.Id,
                        product.Sku,
                        product.Name,
                        product.CurrentUnitPrice,
                        product.StockQuantity ?? 0,
                        product.IsActive)
                    : null;
            }

            public async Task<IEnumerable<ProductModel.Response>> GetProducts()
            {
                var products = await _repository.GetFiltered<Product>(p => p.IsActive);
                return products?.Select(p =>
                    new ProductModel.Response(
                        p.Id,
                        p.Sku,
                        p.Name,
                        p.CurrentUnitPrice,
                        p.StockQuantity ?? 0,
                        p.IsActive)) ?? Enumerable.Empty<ProductModel.Response>();
            }

            public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
            {
             
                if (string.IsNullOrWhiteSpace(request.Sku) ||
                    string.IsNullOrWhiteSpace(request.Name) ||
                    request.CurrentUnitPrice <= 0 || request.StockQuantity < 0)
                {
                    throw new ArgumentException("Datos inválidos para crear el producto");
                }

               
                var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
                if (exist != null)
                    throw new InvalidOperationException($"Ya existe un producto con el SKU '{request.Sku}'");

                    var product = new Product(
                    sku: request.Sku,
                    internalCode: request.InternalCode,
                    name: request.Name,
                    descripcion: request.Description,
                    price: request.CurrentUnitPrice,
                    stock: request.StockQuantity

                );

                await _repository.Add(product);

                return new ProductModel.Response(
                    product.Id,
                    product.Sku,
                    product.Name,
                    product.CurrentUnitPrice,
                    product.StockQuantity ?? 0,
                    product.IsActive
                );
            }

            public async Task<ProductModel.Response?> UpdateProduct(Guid id, ProductModel.Request request)
            {
                var product = await _repository.GetById<Product>(id);
                if (product is null || !product.IsActive) return null;

                product.Sku = request.Sku;
                product.Name = request.Name;
                product.InternalCode = request.InternalCode;
                product.Description = request.Description;
                product.CurrentUnitPrice = request.CurrentUnitPrice;
                product.StockQuantity = request.StockQuantity;

                await _repository.Update(product);

                return new ProductModel.Response(
                    product.Id,
                    product.Sku,
                    product.Name,
                    product.CurrentUnitPrice,
                    product.StockQuantity ?? 0,
                    product.IsActive
                );
            }

            public async Task<bool> DisableProduct(Guid id)
            {
                var product = await _repository.GetById<Product>(id);
                if (product is null || !product.IsActive) return false;

                product.IsActive = false;
                await _repository.Update(product);
                return true;
            }
        }
    }
