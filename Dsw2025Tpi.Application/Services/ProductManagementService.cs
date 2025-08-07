﻿using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;


namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService : IProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository; // inyeccion de dependencia del repositorio, para poder utilizar los metodos que implementa
    }


    public async Task<ProductModel.ProductResponseUpdate>? GetProductById(Guid id) 
    {
        var product = await _repository.GetById<Product>(id); //llama al metodo GetById del repositorio para obtener el producto
        if (product == null) throw new EntityNotFoundException("Producto no encontrado");

        return new ProductModel.ProductResponseUpdate( //mapea la entidad Product a un DTO ProductResponseUpdate
            product.Id,
            product.Sku,
            product.Name,
            product.CurrentUnitPrice,
            product.InternalCode,
            product.Description,
            product.StockQuantity,
            product.IsActive
        );
    }

    public async Task<List<ProductModel.ProductResponseUpdate>?> GetProducts()
    {
        var products = await _repository.GetAll<Product>(); 
        if (products == null || !products.Any() || products.Where(p => p.IsActive) == null) 
        {
            throw new EntityNotFoundException("No se encontraron productos activos.");
        }

        return products.Where(p => p.IsActive == true).Select(p => new ProductModel.ProductResponseUpdate(
        p.Id,
        p.Sku, 
        p.Name,
        p.CurrentUnitPrice,
        p.InternalCode,
        p.Description,
        p.StockQuantity,
        p.IsActive
    )).ToList(); // convierte la lista de productos a una lista de DTOs ProductResponseUpdate
    }


    public async Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || //validad que los campos no esten vacios
            string.IsNullOrWhiteSpace(request.InternalCode) ||
            string.IsNullOrWhiteSpace(request.Description) ||
            string.IsNullOrWhiteSpace(request.Name) ||

            request.StockQuantity < 0) // valida que la cantidad de stock sea mayor a 0
        {
            throw new ArgumentException("Valores de stock, para el producto, no validos");
        }
        if (request.CurrentUnitPrice <= 0) throw new PriceNullException("El precio del producto no puede ser cero o menor.");
        var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
        if (exist != null) throw new DuplicatedEntityException($"Ya existe un producto con el Sku {request.Sku}");

        var product = new Product(request.Sku, request.InternalCode, request.Name, request.Description, request.CurrentUnitPrice, request.StockQuantity);

        await _repository.Add(product);
        return new ProductModel.ProductResponse(product.Id, product.Sku, product.Name, product.CurrentUnitPrice, product.InternalCode, product.Description, product.StockQuantity);
    }   //Devuelve el producto creado como DTO de respuesta.

   
    public async Task<bool> DisableProductAsync(Guid id) 
    {
        var product = await _repository.GetById<Product>(id);
        if (product is null || !product.IsActive)
            throw new EntityNotFoundException("Producto no encontrado o ya deshabilitado.");

        product.IsActive = false;
        await _repository.Update(product);
        return true; 
    }


    public async Task<ProductModel.ProductResponseUpdate> UpdateAsync(ProductModel.ProductRequest request, Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new EntityNotFoundException($"Producto con ID {id} no encontrado.");
        if (request == null ||
                string.IsNullOrWhiteSpace(request.Sku) || 
                string.IsNullOrWhiteSpace(request.InternalCode) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                request.CurrentUnitPrice <= 0)
            throw new ArgumentException("Valores para el producto no validos");

        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CurrentUnitPrice = request.CurrentUnitPrice;
        product.StockQuantity = request.StockQuantity;


        await _repository.Update(product);

        return new ProductModel.ProductResponseUpdate(
            product.Id,
            product.Sku,
            product.Name,
            product.CurrentUnitPrice,
            product.InternalCode,
            product.Description,
            product.StockQuantity,
            product.IsActive
        );
    }

}
