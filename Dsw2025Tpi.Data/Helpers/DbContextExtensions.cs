using Dsw2025Tpi.Domain.Entities;
using System.Text.Json;

namespace Dsw2025Tpi.Data.helpers
{
    public static class DbContextExtensions
    {
        public static void Seedwork<T>(this Dsw2025TpiContext context, string dataSource) where T : EntityBase
        {
            if (context.Set<T>().Any()) return;
            var jsonPath = Path.Combine(AppContext.BaseDirectory, dataSource);
            if (!File.Exists(jsonPath)) return;

            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, dataSource));
            var entities = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            if (entities == null || entities.Count == 0) return;

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

            context.SaveChanges();
        }
    }
}