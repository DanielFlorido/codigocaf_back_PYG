using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Log;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.BalanceGeneral;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BalanceGeneralController : ControllerBase
{

    private static string CONTROLLER_NAME = "BalanceGeneralController";

    private readonly IBalanceGeneralService _balanceGeneralService;

    private readonly ILogService _logService;

    public BalanceGeneralController(IBalanceGeneralService balanceGeneralService, ILogService logService)
    {
        _balanceGeneralService = balanceGeneralService;
        _logService = logService;
    }

    [HttpGet("Anios")]
    public IActionResult ListarAnios([FromHeader(Name = "x-client-id")] int clienteId)
    {
        try
        {
            return Ok(_balanceGeneralService.ListarAnioBalanceGeneral(clienteId));
        } catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "ListarAnios", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    [HttpPost("Encabezado")]
    public IActionResult ConsultarEncabezado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_balanceGeneralService.ConsultarEncabezado(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "Encabezado", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    [HttpPost("SubEncabezado")]
    public IActionResult ConsultarSubEncabezado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_balanceGeneralService.ConsultarSubEncabezado(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "SubEncabezado", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    [HttpPost("Detalle")]
    public IActionResult ConsultarDetalle([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_balanceGeneralService.ConsultarDetalle(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "ConsultarDetalle", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    [HttpPost("Tercero")]
    public IActionResult ConsultarTercero([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        try
        {
            return Ok(_balanceGeneralService.ConsultarTercero(clienteId, request));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "ConsultarTercero", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    [HttpPost("EncabezadoAgrupado")]
    public IActionResult ConsultarEncabezadoAgrupado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return Ok(_balanceGeneralService.ConsultarEncabezadoAgrupado(clienteId, balanceGeneralAnioRequest));
    }

    [HttpPost("SubEncabezadoAgrupado")]
    public IActionResult ConsultarSubEncabezadoAgrupado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return Ok(_balanceGeneralService.ConsultarSubEncabezadoAgrupado(clienteId, balanceGeneralAnioRequest));
    }

    [HttpPost("DetalleAgrupado")]
    public IActionResult ConsultarDetalleAgrupado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return Ok(_balanceGeneralService.ConsultarDetalleAgrupado(clienteId, balanceGeneralAnioRequest));
    }

    [HttpPost("TerceroAgrupado")]
    public IActionResult ConsultarTerceroAgrupado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return Ok(_balanceGeneralService.ConsultarTerceroAgrupado(clienteId, balanceGeneralAnioRequest));
    }

    [HttpPost("GuardarNota")]
    public IActionResult GuardarNota([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] NotaBalanceGeneralRequest notaBalanceGeneralRequest)
    {
        return Ok(_balanceGeneralService.GuardarNota(clienteId, notaBalanceGeneralRequest));
    }


    [HttpPost("Descargar/Excel")]
    public IActionResult DescargarExcel([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        return Ok(Convert.ToBase64String(_balanceGeneralService.GenerarExcel(clienteId, request)));
    }

    [HttpPost("Descargar/ExcelAgrupado")]
    public IActionResult DescargarExcelAgrupado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return Ok(Convert.ToBase64String(_balanceGeneralService.GenerarExcelAgrupado(clienteId, balanceGeneralAnioRequest)));
    }

    [HttpPost("EnviarExcel")]
    public IActionResult EnviarExcel([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralRequest request)
    {
        _balanceGeneralService.EnviarExcel(clienteId, request);
        return Ok(new MensajeResultado() { Mensaje = "Se ha enviado el reporte al correo" });
    }

    [HttpPost("EnviarExcelAgrupado")]
    public IActionResult EnviarExcelAgrupado([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        _balanceGeneralService.EnviarExcelAgrupado(clienteId, balanceGeneralAnioRequest);
        return Ok(new MensajeResultado() { Mensaje = "Se ha enviado el reporte al correo" });
    }

}
