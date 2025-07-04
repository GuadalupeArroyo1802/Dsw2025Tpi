﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Data.Repositories
{
    public class InMemoryRepository : IRepository
    {
        private readonly List<Product> _products = new();
        private readonly List<Customer> _customers = new();

        public InMemoryRepository()
        {
            LoadProducts();
            LoadCustomers();
        }

        private void LoadProducts()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Sources", "products.json");
            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                var list = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (list != null) _products.AddRange(list);
            }
        }

        private void LoadCustomers()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Sources", "customers.json");
            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                var list = JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (list != null) _customers.AddRange(list);
            }
        }

        private List<T>? GetList<T>() where T : EntityBase
        {
            return typeof(T).Name switch
            {
                nameof(Product) => _products as List<T>,
                nameof(Customer) => _customers as List<T>,
                _ => throw new NotSupportedException()
            };
        }

        public Task<T?> GetById<T>(Guid id, params string[] include) where T : EntityBase
        {
            return Task.FromResult(GetList<T>()?.FirstOrDefault(e => e.Id == id));
        }

        public Task<IEnumerable<T>?> GetAll<T>(params string[] include) where T : EntityBase
        {
            return Task.FromResult(GetList<T>()?.AsEnumerable());
        }

        public Task<T?> First<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
        {
            return Task.FromResult(GetList<T>()?.FirstOrDefault(predicate.Compile()));
        }

        public Task<IEnumerable<T>?> GetFiltered<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
        {
            return Task.FromResult(GetList<T>()?.Where(predicate.Compile()));
        }

        public Task<T> Add<T>(T entity) where T : EntityBase
        {
            GetList<T>()?.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<T> Update<T>(T entity) where T : EntityBase
        {
            var list = GetList<T>();
            var existing = list?.FirstOrDefault(e => e.Id == entity.Id);
            if (existing != null)
            {
                list!.Remove(existing);
                list.Add(entity);
            }
            return Task.FromResult(entity);
        }

        public Task<T> Delete<T>(T entity) where T : EntityBase
        {
            GetList<T>()?.Remove(entity);
            return Task.FromResult(entity);
        }
    }
}