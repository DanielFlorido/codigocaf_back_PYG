using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.ProgramadorPago;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProgramadorPagoController : ControllerBase
{

    private readonly IProgramadorPagoService _programadorPagoService;

    public ProgramadorPagoController(IProgramadorPagoService programadorPagoService)
    {
        _programadorPagoService = programadorPagoService;
    }

    [HttpPost("Consultar")]
    public IActionResult Consultar([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest)
    {
        return Ok(_programadorPagoService.ListarProgramador(clienteId, consultarBuscadorPagoRequest));
    }

    [HttpPost("GenerarCSV")]
    public IActionResult GenerarCSV([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest)
    {
        return Ok(_programadorPagoService.GenerarCSV(clienteId, consultarBuscadorPagoRequest));
    }


}
