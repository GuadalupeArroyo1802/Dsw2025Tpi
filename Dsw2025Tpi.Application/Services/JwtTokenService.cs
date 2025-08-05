using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dsw2025Tpi.Application.Services;

public class JwtTokenService
{
    private readonly IConfiguration _config; 

    // Constructor con inyección de la configuración (IConfiguration accede a appsettings.json)
    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(string username, string role)
    {
        // Obtiene la sección "Jwt" del archivo de configuración (appsettings.Deveplopment.json)
        var jwtConfig = _config.GetSection("Jwt");

        // Lee la clave secreta usada para firmar el token (debe estar en el config)
        var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt Key");

        // Convierte la clave a bytes y la empaqueta en un objeto de seguridad simétrica
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));

        // Crea las credenciales de firma usando el algoritmo HMAC-SHA256
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Crea una lista de "claims" (información que contien el token)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),                
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            new Claim("role", role)                                           
        };

        // Crea el token JWT con los parámetros especificados
        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],                  // Emisor del token 
            audience: jwtConfig["Audience"],              // Audiencia esperada 
            claims: claims,                               // Información codificada (claims)
            expires: DateTime.Now.AddMinutes(             // Tiempo de expiración del token
                double.Parse(jwtConfig["ExpireInMinutes"] ?? "60")),
            signingCredentials: creds                     // Firma del token
        );

        return new JwtSecurityTokenHandler().WriteToken(token); // Serializa el token a una cadena para enviar al usuario
    }
}