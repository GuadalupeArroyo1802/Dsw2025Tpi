
using Microsoft.AspNetCore.DataProtection.Repositories;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Data.Sources;
using Dsw2025Tpi.Data.Helpers;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        builder.Services.AddScoped<IRepository, EfRepository>();
        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiContextEntities"));
            options.UseSeeding((c, t) =>
            {
               
                ((Dsw2025TpiContext)c).Seedwork<Product>("Sources\\products.json");
                ((Dsw2025TpiContext)c).Seedwork<Customer>("Sources\\categories.json");
                ((Dsw2025TpiContext)c).Seedwork<Order>("Sources\\orders.json");
                ((Dsw2025TpiContext)c).Seedwork<OrderItem>("Sources\\orders.json");
            });
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/healthcheck");

        app.Run();
    }
}
