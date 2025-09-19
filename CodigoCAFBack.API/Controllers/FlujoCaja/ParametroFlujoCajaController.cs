using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CodigoCAFBack.API.Controllers.FlujoCaja;

[Route("api/[controller]")]
[ApiController]
public class ParametroFlujoCajaController : ControllerBase
{
    private readonly IParametroFlujoCajaService _parametroFlujoCajaService;

    public ParametroFlujoCajaController(IParametroFlujoCajaService parametroFlujoCajaService)
    {
        _parametroFlujoCajaService = parametroFlujoCajaService;
    }

    [HttpGet]
    public IActionResult GetParametro([FromHeader(Name = "x-client-id")] int clienteId)
    {
        return Ok(_parametroFlujoCajaService.ListarParametros(clienteId));
    }

    [HttpPost]
    public IActionResult Crear([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] ParametroFlujoCajaRequest parametroFlujoCaja)
    {
        parametroFlujoCaja.IdCliente = idCliente;
        return Ok(_parametroFlujoCajaService.CrearParametro(parametroFlujoCaja));
    }


    [HttpPut("{id}")]
    public IActionResult Editar(Int32 id, [FromHeader(Name = "x-client-id")] int idCliente, [FromBody] ParametroFlujoCajaRequest parametroFlujoCaja)
    {
        parametroFlujoCaja.Id = id;
        parametroFlujoCaja.IdCliente = idCliente;
        return Ok(_parametroFlujoCajaService.EditarParametro(parametroFlujoCaja));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(Int32 id)
    {
        _parametroFlujoCajaService.ElimiarParametro(id);
        return Ok(new MensajeResultado() { Mensaje = "Se ha eliminado parámetro, exitosamente" });
    }
}
