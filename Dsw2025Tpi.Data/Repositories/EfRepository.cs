using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore; //libreria para manejar la bdd

namespace Dsw2025Tpi.Data.Repositories;

public class EfRepository : IRepository
{
    private readonly Dsw2025TpiContext _context;

    public EfRepository(Dsw2025TpiContext context)
    {
        _context = context; //se realiza la inyeccion del contexto, para poder trabajar con los datos en la base de datos
    }

    public async Task<T> Add<T>(T entity) where T : EntityBase  //metodo generico para agregar una entidad a la bdd
    {
        await _context.AddAsync(entity); //agrega la entidad al contexto de la bdd
        await _context.SaveChangesAsync(); //guarda los cambios de manera asincrona
        return entity; //retorna la entidad agregada
    }

    public async Task<T> Delete<T>(T entity) where T : EntityBase 
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return entity; 
    }

    //devuelve un una entidad que cumpla con la condicion de filtrado, inluyendo un parametro array con nombres de navegacion
    public async Task<T?> First<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
    {
        return await Include(_context.Set<T>(), include).FirstOrDefaultAsync(predicate);
    }
    
    //devuelve todas las entidades de un tipo T, incluyendo un parametro array con nombres de navegacion
    public async Task<IEnumerable<T>?> GetAll<T>(params string[] include) where T : EntityBase
    {
        return await Include(_context.Set<T>(), include).ToListAsync(); //convitiendo todo en una lista de manera asincrona
    }

    //devuelve una entidad por su Id, incluyendo un parametro array con nombres de navegacion
    public async Task<T?> GetById<T>(Guid id, params string[] include) where T : EntityBase
    {
        return await Include(_context.Set<T>(), include).FirstOrDefaultAsync(e => e.Id == id);
    }

    //devuelve una lista con entidades filtradas por una condicion, incluyendo un parametro array con nombres de navegacion
    public async Task<IEnumerable<T>?> GetFiltered<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
    {
        return await Include(_context.Set<T>(), include).Where(predicate).ToListAsync();
    }

    // Actualiza una entidad en la base de datos y guarda los cambios
    public async Task<T> Update<T>(T entity) where T : EntityBase
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }


    // Metodo privado que incluye las propiedades de navegacion en la consulta
    // Este metodo que recibe y trae una consulta IQueryable y un array de strings con los nombres de las propiedades de navegacion
    // y retorna una consulta IQueryable con las propiedades de navegacion incluidas
    private static IQueryable<T> Include<T>(IQueryable<T> query, string[] includes) where T : EntityBase 
    {
        var includedQuery = query; //crea una copia de la consulta original

        foreach (var include in includes) // itera sobre cada nombre de navegacion en el array
        {
            includedQuery = includedQuery.Include(include); // agrega la navegacion(nombre) a la consulta
        }
        return includedQuery; 
    }
}