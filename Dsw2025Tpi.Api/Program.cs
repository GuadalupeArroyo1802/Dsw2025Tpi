using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Dsw2025Tpi.Data.helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Dsw2025Tpi.Application.Services;



namespace Dsw2025Tpi.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); //objeto que se encarga de configurar la aplicacion

        // Add services to the container.

        builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });
        // https://aka.ms/aspnetcore/swashbuckle guia a un recurso para saber como configurar Swagger/OpenAPI
      
        builder.Services.AddScoped<IProductsManagementService, ProductsManagementService>();
        builder.Services.AddScoped<IOrderManagement, OrderManagement>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddTransient<IRepository, EfRepository>();
        builder.Services.AddSwaggerGen(o => //Creacion de la documentacion de la API con Swagger
        {
            o.SwaggerDoc("v1", new OpenApiInfo ///informacion de la API
            {
                Title = "Desarrollo De Software",
                Version = "v1", 
            });
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme //definicion de esquema de seguridad
            {
                In = ParameterLocation.Header, //define donde se espera el token
                Name = "Authorization", //Define el nombre del header donde se env�a el token.
                Description = "Ingresar el token", //especifica donde va el token
                Type = SecuritySchemeType.ApiKey, //Indica que Swagger usar� este esquema como si fuera una token Bearer.
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement //indica que todos los endpoints requieren autenticacion
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            //Esto es una referencia al esquema "Bearer" que definiste antes con AddSecurityDefinition. Lo en laza
                        }
                    },
                    new string[] {} //solamente se lo instancia, no se lo utiliza por que no utilizamos alcances especificos para acceder al recurso
                }
            });
        });
        builder.Services.AddDbContext<Dsw2025TpiContext>(options => //registro del contexto y representa la bdd
        {

            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Dsw2025TpiDB;Integrated Security=True");
            //Este DbContext debe usar SQL Server como proveedor de base de datos
        });
        builder.Services.AddHealthChecks();

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => //configuracion de identidad
        {
            options.Password = new PasswordOptions //reglas de la contrase�a
            {
                RequiredLength = 8

            };
        })
        .AddEntityFrameworkStores<AuthenticateContext>() //EF proveedor de almacenamiento, usando la clase AutheticateContext
        .AddDefaultTokenProviders(); //proporciona los tokens por defecto para la autenticacion y autorizacion


        var jwtConfig = builder.Configuration.GetSection("Jwt"); //busca la seccion Jwt en el archivo de configuracion
        var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt Key"); //obtiene la clave de la configuracion, si no existe lanza una excepcion
        var key = Encoding.UTF8.GetBytes(keyText); // convierte la clave a un arreglo de bytes

        builder.Services.AddAuthentication(options => //se establecen los esquemas que se van a utilizar para la autenticacion
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters //reglas que se aplican al token JWT
               {
                   ValidateIssuer = true, //valida el emisor del token
                   ValidateAudience = true, //valida el destinatario del token
                   ValidateLifetime = true, // valida la fecha de expiracion del token
                   ValidateIssuerSigningKey = true, //valida que el token no haya sido modificado
                   ValidIssuer = jwtConfig["Issuer"], 
                   ValidAudience = jwtConfig["Audience"], 
                   IssuerSigningKey = new SymmetricSecurityKey(key) // se usa para validar la firma del token.

               };
           });

        builder.Services.AddSingleton<JwtTokenService>(); //se intancia la clase de servicio de token JWT una sola vez durante toda la vidade la app

        builder.Services.AddDbContext<AuthenticateContext>(options => //se configura el contexto de autenticacion
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiEntities")); //es la base de datos de seguridad/autenticaci�n de la aplicaci�n.

            //obtiene la cadena de coneccion desde el archivo de configuracion, evitando harcodear las cadenas en el codigo fuente
        });


        builder.Services.AddCors(options => //se configura CORS para permitir solicitudes desde el frontend, evitando solicitudes de origen cruzado
        {
            options.AddPolicy("PermitirFrontend", policy =>
                policy.WithOrigins("http://localhost:3000") //especifica el origen permitido, en este caso el frontend de React
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });

        var app = builder.Build(); // se crear la app con todas las configuraciones y servicios que se han registrado
        using (var scope = app.Services.CreateScope()) //se crea un ambito de ejecucion para poder acceder a los servicios registrados
        {
            var db = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>(); //establece una instancia del contexto de la base de datos
            db.Seedwork<Customer>(Path.Combine(AppContext.BaseDirectory, "Sources", "customers.json"));
            db.Seedwork<Product>(Path.Combine(AppContext.BaseDirectory, "Sources", "products.json"));
            db.Seedwork<Order>(Path.Combine(AppContext.BaseDirectory, "Sources", "orders.json"));

            //AppContext.BaseDirectory es la carpeta donde se ejecuta la aplicaci�n, y "Sources" es una carpeta
            //dentro de esa ubicaci�n que contiene los archivos JSON con los datos iniciales.

            //Path.Combine() construye la ruta a los archivos de datos dentro de la carpeta Sources.
           
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //se crea una instancia de RolemManager para manejar los roles de identidad
            string[] roles = new[] { "admin", "customer" }; //solo son estos dos roles que se van a utilizar en la aplicacion

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role)) //verifica si el rol ya existe
                {
                    await roleManager.CreateAsync(new IdentityRole(role)); //si no existe, lo crea
                }
            }
        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) //verifica si la aplicacion esta en modo desarrollo
        {
            app.UseSwagger(); //si la aplicacion esta en desarrollo, se habilita Swagger para documentar la API
            app.UseSwaggerUI(); //se habilita la interfaz de usuario de Swagger para interactuar con la API
        }
        // middleware es un componente que intercepta y maneja las solicitudes HTTP.
        app.UseHttpsRedirection(); //redirecciona las solicitudes HTTP a HTTPS

        app.UseAuthentication(); //se habilita la autenticacion, permite que los usuarios se autentiquen con JWT

        app.UseAuthorization(); //se habilita la autorizacion, permite que los usuarios accedan a los recursos protegidos de la API

        app.MapControllers(); //mapea los controladores de la API, permite que las solicitudes HTTP se dirijan a los controladores correspondientes

        app.MapHealthChecks("/healthcheck"); //mapea el endpoint de verificacion de salud de la aplicacion, permite verificar si la aplicacion esta funcionando correctamente

        app.Run(); //inicia la aplicacion y comienza a escuchar las solicitudes HTTP entrantes
    }
}