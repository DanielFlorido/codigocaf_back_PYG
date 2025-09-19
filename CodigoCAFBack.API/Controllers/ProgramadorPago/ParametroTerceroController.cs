using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.ProgramadorPago;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ParametroTerceroController : ControllerBase
{

    private readonly IParametroTerceroService _parametroTerceroService;

    public ParametroTerceroController(IParametroTerceroService parametroTerceroService)
    {
        _parametroTerceroService = parametroTerceroService;
    }

    [HttpGet]
    public IActionResult GetTerceros([FromHeader(Name = "x-client-id")] int clienteId)
    {
        return Ok(_parametroTerceroService.ListarParametros(clienteId));
    }

    [HttpGet("{language}")]
    public IActionResult ObtenerTipoProveedor([FromHeader(Name = "x-client-id")] int clienteId, string language)
    {
        return Ok(_parametroTerceroService.ListarTipoProveedor(clienteId, language));
    }

    [HttpPost]
    public IActionResult Crear([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] ParametroTerceroRequest parametroTercero)
    {
        parametroTercero.IdCliente = idCliente;
        return Ok(_parametroTerceroService.CrearParametro(parametroTercero));
    }


    [HttpPut("{id}")]
    public IActionResult Editar(Int32 id, [FromHeader(Name = "x-client-id")] int idCliente, [FromBody] ParametroTerceroRequest parametroTercero)
    {
        parametroTercero.Id = id;
        parametroTercero.IdCliente = idCliente;
        return Ok(_parametroTerceroService.EditarParametro(parametroTercero));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(Int32 id)
    {
        _parametroTerceroService.ElimiarParametro(id);
        return Ok(new MensajeResultado() { Mensaje = "Se ha eliminado parámetro, exitosamente" });
    }

}
