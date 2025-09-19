using CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.FlujoCaja;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FlujoCajaController : ControllerBase
{

    private readonly IFlujoCajaService _flujoCajaService;

    public FlujoCajaController(IFlujoCajaService flujoCajaService)
    {
        _flujoCajaService = flujoCajaService;
    }

    [HttpGet("Anios")]
    public IActionResult ListarAnios([FromHeader(Name = "x-client-id")] int idCliente)
    {
        return Ok(_flujoCajaService.ListarAnio(idCliente));
    }

    [HttpPost("Encabezado")]
    public IActionResult Encabezado([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] FlujoCajaRequest flujoCajaRequest)
    {
        return Ok(_flujoCajaService.ConsultarEncabezado(idCliente, flujoCajaRequest));
    }

    [HttpPost("SubEncabezado")]
    public IActionResult SubEncabezado([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] FlujoCajaRequest flujoCajaRequest)
    {
        return Ok(_flujoCajaService.ConsultarSubEncabezado(idCliente, flujoCajaRequest));
    }

    [HttpPost("DetalleTercero")]
    public IActionResult DetalleTercero([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] FlujoCajaRequest flujoCajaRequest)
    {
        return Ok(_flujoCajaService.DetalleTercero(idCliente, flujoCajaRequest));
    }

    [HttpGet("ListarTercero")]
    public IActionResult ListarTercero([FromHeader(Name = "x-client-id")] int idCliente)
    {
        return Ok(_flujoCajaService.BuscarTercero(idCliente));
    }

    [HttpPost("DescargarExcel")]
    public IActionResult GenerarExcel([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] FlujoCajaRequest flujoCajaRequest)
    {
        return Ok(Convert.ToBase64String(_flujoCajaService.GenerarExcel(idCliente, flujoCajaRequest)));
    }

    [HttpPost("EnviarExcel")]
    public IActionResult EnviarExcel([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] FlujoCajaRequest flujoCajaRequest)
    {
        _flujoCajaService.GenerarExcel(idCliente, flujoCajaRequest);
        return Ok(new MensajeResultado() { Mensaje = "Se ha enviado el reporte al correo" });
    }

}
