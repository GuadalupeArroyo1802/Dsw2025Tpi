using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data
{
    public class AuthenticateContext : IdentityDbContext //creando automaticamente una base de datos con tablas 
    {
        public AuthenticateContext(DbContextOptions<AuthenticateContext> options) : base(options)
        {
            //se recibe las opciones de configuracion de la base de datos en el contructor
        }
        protected override void OnModelCreating(ModelBuilder builder)
        //este metodo se usa para configurar el modelo de datos y las tablas que se van a crear en la base de datos
        {//se usa override para modificar el comportamiento del modelo de datos

            base.OnModelCreating(builder); //llama al metodo original para no perder la configuracion por defecto
            //se personalizan los nombres de las tablas de identidad
            builder.Entity<IdentityUser>(b => { b.ToTable("Usuarios"); });
            builder.Entity<IdentityRole>(b => { b.ToTable("Roles"); });
            builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UsuariosRoles"); });
            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UsuariosClaims"); });
            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UsuariosLogins"); });
            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RolesClaims"); });
            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UsuariosTokens"); });
        }
    }
}
