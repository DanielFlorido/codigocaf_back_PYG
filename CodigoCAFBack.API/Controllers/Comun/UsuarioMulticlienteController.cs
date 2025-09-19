using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Dominio.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.Comun;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsuarioMulticlienteController : ControllerBase
{
    private static string CONTROLLER_NAME = "UsuarioMulticlienteController";

    private readonly IUsuarioMulticlienteService _usuarioMulticlienteService;

    private readonly ILogService _logService;

    public UsuarioMulticlienteController(IUsuarioMulticlienteService usuarioMulticlienteService, ILogService logService)
    {
        _usuarioMulticlienteService = usuarioMulticlienteService;
        _logService = logService;
    }

    [HttpGet("{usuarioId}")]
    public IActionResult ConsultarClientesPorUsuario(Int32 usuarioId)
    {
        try
        {
            return Ok(_usuarioMulticlienteService.ConsultarClientesPorUsuario(usuarioId));

        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "ListarParametros", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

}
