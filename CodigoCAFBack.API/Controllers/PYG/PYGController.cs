using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Aplicacion.Interfaces.PYG;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.PYG;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PYGController  : ControllerBase
{
    private readonly IPYGService _pygService;
    private readonly ILogService _logService;
    private static string CONTROLLER_NAME = "PYGController";
    public PYGController(IPYGService pYGService, ILogService logService)
    {
        _pygService = pYGService;
        _logService = logService;
    }

    [HttpPost("ConsultarPYGGrupos")]
    public IActionResult ConsultarPYGGrupos([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_pygService.ConsultarPYGGrupos(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Dominio.Log.Log() { Controlador = CONTROLLER_NAME, Metodo = "ConsultarPYG", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }
    [HttpPost("ConsultarPYGCuentas")]
    public IActionResult ConsultarPYGCuentas([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_pygService.ConsultarPYGCuentas(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Dominio.Log.Log() { Controlador = CONTROLLER_NAME, Metodo = "ConsultarPYG", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }
    [HttpPost("ConsultarPYGSubCuentas")]
    public IActionResult ConsultarPYGSubCuentas([FromHeader(Name = "x-client-id")]int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_pygService.ConsultarPYGSubCuentas(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Dominio.Log.Log() { Controlador = CONTROLLER_NAME, Metodo = "ConsultarPYG", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }
    [HttpPost("ConsultarPYGAuxiliar")]
    public IActionResult ConsultarPYGAuxiliar([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_pygService.ConsultarPYGAuxiliares(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Dominio.Log.Log() { Controlador = CONTROLLER_NAME, Metodo = "ConsultarPYG", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    [HttpPost("Descargar/PYGExcel")]
    public IActionResult DescargarPYGExcel([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_pygService.GenerarExcel(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Dominio.Log.Log() { Controlador = CONTROLLER_NAME, Metodo = "DescargarPYGExcel", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }
    [HttpPost("Enviar/PYGExcel")]
    public IActionResult EnviarPYGExcel([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            _pygService.EnviarExcel(clienteId, request);
            return Ok();
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Dominio.Log.Log() { Controlador = CONTROLLER_NAME, Metodo = "EnviarPYGExcel", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }
}
