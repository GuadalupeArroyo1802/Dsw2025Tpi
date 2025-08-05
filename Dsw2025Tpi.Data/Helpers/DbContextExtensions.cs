using Dsw2025Tpi.Domain.Entities;
using System.Text.Json;

namespace Dsw2025Tpi.Data.helpers
{
    public static class DbContextExtensions
    {
        public static void Seedwork<T>(this Dsw2025TpiContext context, string dataSource) where T : EntityBase
        {
            if (context.Set<T>().Any()) return;//Evita duplicacion (si ya hay datos, salir)
            var jsonPath = Path.Combine(AppContext.BaseDirectory, dataSource);
            if (!File.Exists(jsonPath)) return;

            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, dataSource)); //Lee el json
            var entities = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {//Deserializa el contenido del .json, ignorando mayus. y minus. en propiedades
                PropertyNameCaseInsensitive = true,
            });
            if (entities == null || entities.Count == 0) return; //Si no se deserializa, o lista vacía, salir
                                                                 //context.Set<T>().AddRange(entities);
            foreach (var entity in entities)
            {
                var entityId = ((EntityBase)entity).Id;

                var alreadyTracked = context.Set<T>().Local
                    .Cast<EntityBase>()
                    .Any(e => e.Id == entityId);

                if (!alreadyTracked)
                {
                    context.Set<T>().Add(entity);
                }
            }

            context.SaveChanges(); //Guarda los cambios
        }
    }
}