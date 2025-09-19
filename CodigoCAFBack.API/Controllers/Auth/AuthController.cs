
using CodigoCAFBack.Dominio.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodigoCAFBack.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly IConfiguration _configuration;

    private readonly List<UsuarioLogin> _users;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
        _users = _configuration.GetSection("UsuariosValidados").Get<List<UsuarioLogin>>();
    }

    [HttpPost("token")]
    public IActionResult GenerarToken([FromBody] UsuarioLogin usuarioLogin)
    {
         var user = _users.Find(x => x.Usuario == usuarioLogin.Usuario && x.Clave == usuarioLogin.Clave);
        // Validar las credenciales del usuario (esto es solo un ejemplo)
        if (user == null)
        {
            return Unauthorized("Credenciales inválidas");
        }

        // Validar que las configuraciones necesarias no sean nulas
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Configuración de JWT faltante");
        }

        // Crear los claims del token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Usuario),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("UserId", user.UserId)
        };

        // Generar la clave de firma
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Crear el token
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.Now.AddHours(10),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    [HttpGet("Token")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Token no proporcionado");  
        }
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("El token no contiene un UserId válido");
        }

        return Ok(new InfoUsuario()
        {
            Usuario = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value ?? "Usuario desconocido",
            IdUsuario = userId
        });
    }

}
