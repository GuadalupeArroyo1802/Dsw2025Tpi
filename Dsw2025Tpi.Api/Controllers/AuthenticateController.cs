using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager; 
    private readonly SignInManager<IdentityUser> _signInManager; 
    private readonly JwtTokenService _jwtTokenService; 

    public AuthenticateController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;    
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        var user = await _userManager.FindByNameAsync(request.Username); 
        if (user == null)
        {
            return Unauthorized("Usuario incorrecto o inexistente");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) 
        {
            return Unauthorized("Contraseña incorrecta");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles == null || roles.Count == 0)
        {
            return Unauthorized("El usuario no tiene un rol asignado.");
        }
        var role = roles.First();
        var token = _jwtTokenService.GenerateToken(request.Username, role);
        return Ok(new { token });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            var emailCheck = new System.Net.Mail.MailAddress(model.Email); 
        }
        catch (FormatException)
        {
            return BadRequest("El correo electrónico tiene un formato inválido.");
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest("El correo electrónico ya está registrado.");
        }

        var role = string.IsNullOrWhiteSpace(model.Role) ? "customer" : model.Role.ToLower(); 
        if (role != "admin" && role != "customer")
        {
            return BadRequest("Rol inválido. Solo se permiten 'admin' o 'customer'.");
        }

        var user = new IdentityUser { UserName = model.Username, Email = model.Email }; 
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, role);

        return Ok("Usuario registrado correctamente.");
    }
}