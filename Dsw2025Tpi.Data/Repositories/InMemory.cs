using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

namespace Dsw2025Tpi.Data.Repositories
{
    public class InMemoryRepository : IRepository
    {
        private readonly List<Product> _products = new();
        private readonly List<Customer> _customers = new();

        public InMemoryRepository()
        {
            LoadProducts(); //cargar productos desde el archivo JSON
            LoadCustomers(); //cargar clientes desde el archivo JSON
        }

        private void LoadProducts()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Sources", "products.json");
            if (File.Exists(jsonPath)) //verifica si el archivo existe
            {
                var json = File.ReadAllText(jsonPath); //lee todo el contenido del archivo JSON
                var list = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions //deserializa el contenido JSON a una lista de productos
                {
                    PropertyNameCaseInsensitive = true //ignora mayusculas y minusculas en los nombres de las propiedades
                });
                if (list != null) _products.AddRange(list); //si la lista no es nula, agrega los productos a la lista interna
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

        private List<T>? GetList<T>() where T : EntityBase //obtener la lista correspondiente segun el tipo de entidad
        {
            return typeof(T).Name switch //operador switch basado en el nombre del tipo T
            {
                nameof(Product) => _products as List<T>, //si T es Product, retorna la lista de productos
                nameof(Customer) => _customers as List<T>, //si T es Customer, retorna la lista de clientes
                _ => throw new NotSupportedException() //si T no es ninguno de los anteriores, lanza una excepcion indicando que el tipo no es soportado
            };
        }

        public Task<T?> GetById<T>(Guid id, params string[] include) where T : EntityBase // obtener una entidad por su Id
        {
            return Task.FromResult(GetList<T>()?.FirstOrDefault(e => e.Id == id)); //busca en la lista correspondiente la entidad con el Id especificado y la retorna
        }

        public Task<IEnumerable<T>?> GetAll<T>(params string[] include) where T : EntityBase // obtener todas las entidades de un tipo T
        {
            return Task.FromResult(GetList<T>()?.AsEnumerable()); //retorna la lista correspondiente convertida a IEnumerable, que se puede recorrer
        }

        public Task<T?> First<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase // obtener la primera entidad que cumpla con una condicion
        {
            return Task.FromResult(GetList<T>()?.FirstOrDefault(predicate.Compile())); //busca en la lista correspondiente la primera entidad que cumpla con el predicado y la retorna
        }

        public Task<IEnumerable<T>?> GetFiltered<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase // obtener una lista de entidades filtradas por una condicion
        {
            return Task.FromResult(GetList<T>()?.Where(predicate.Compile())); //filtra la lista correspondiente usando el predicado y retorna las entidades que cumplan con la condicion
        }

        public Task<T> Add<T>(T entity) where T : EntityBase // agregar una nueva entidad a la lista correspondiente
        {
            GetList<T>()?.Add(entity); //agrega la entidad a la lista correspondiente
            return Task.FromResult(entity); //retorna la entidad agregada
        }

        public Task<T> Update<T>(T entity) where T : EntityBase // actualizar una entidad existente en la lista correspondiente
        {
            var list = GetList<T>(); // obtiene la lista correspondiente
            var existing = list?.FirstOrDefault(e => e.Id == entity.Id); //busca la entidad existente en la lista por su Id
            if (existing != null) 
            {
                list!.Remove(existing); //si la entidad existe, la elimina de la lista
                list.Add(entity); //y luego agrega la entidad actualizada
            }
            return Task.FromResult(entity); //retorna la entidad actualizada
        }

        public Task<T> Delete<T>(T entity) where T : EntityBase // eliminar una entidad de la lista correspondiente
        {
            GetList<T>()?.Remove(entity); // elimina la entidad de la lista correspondiente
            return Task.FromResult(entity); //retorna la entidad eliminada
        }
    }
}
