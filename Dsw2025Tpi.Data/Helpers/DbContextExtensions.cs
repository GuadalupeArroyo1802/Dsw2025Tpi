using System.Text.Json;


namespace Dsw2025Tpi.Data.helpers
{
    public static class DbContextExtensions
    {
        public static void Seedwork<T>(this Dsw2025TpiContext context, string dataSource) where T : class //puede trabaja con cualqui tipo de clase
        {
            if (context.Set<T>().Any()) return;
            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, dataSource));
            var entities = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // permite que los nombres de las propiedades no sean sensibles a mayúsculas y minúsculas
            });
            if (entities == null || entities.Count == 0) return; // Si no hay entidades deserializadas, no hace nada
            context.Set<T>().AddRange(entities); //Agrega todos los objetos deserializados al contexto de EF Core
            context.SaveChanges(); //Guarda los cambios en la base de datos
        }
    }

    //Esta extension sirve para inicializar la base de datos con datos de ejemplo desde un archivo JSON.
    //Es un metodo generico que lee los .json,  que recibe como parametro la instnacia del DbContext y data sources que es la 
    //ruta del archivo JSON que contiene los datos a cargar.
    //revisa si ya hay datos en la tabla correspondiente a T, y si ya hay no hace nada para evitar duplicados
    //en el caso que no haya datos, lee el archivo .josn, desde el disrectorio de la app y el nombre del archivo
    // deserializa el contenido del archivo a una lista de entidades del tipo T.
}

