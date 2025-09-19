using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Entidades;
using CodigoCAFBack.Dominio.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CodigoCAFBack.API.Controllers.BalanceGeneral;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ParametroBalanceGeneralController : ControllerBase
{

    private static string CONTROLLER_NAME = "ParametroBalanceGeneralController";

    private readonly IParametrosBalanceGeneralService _parametrosBalanceGeneralService;

    private readonly ILogService _logService;

    public ParametroBalanceGeneralController(IParametrosBalanceGeneralService parametrosBalanceGeneralService, ILogService logService)
    {
        _parametrosBalanceGeneralService = parametrosBalanceGeneralService;
        _logService = logService;
    }


    // GET: api/<ParametroBalanceGeneralController>
    [HttpGet]
    public IActionResult ListarParametros([FromHeader(Name = "x-client-id")] int clienteId)
    {
        try { 
            return Ok(_parametrosBalanceGeneralService.ListarParametros(clienteId));
        } 
        catch(Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "ListarParametros", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    // GET api/<ParametroBalanceGeneralController>/padre/5
    [HttpGet("padre/{idPadre:int?}")]
    public IActionResult ListarParametrosPorIdPadre([FromHeader(Name = "x-client-id")] int clienteId, Nullable<Int32> idPadre)
    {
        try {
            return Ok(_parametrosBalanceGeneralService.ListarParametrosPorIdPadre(clienteId, idPadre));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "ListarParametrosPorIdPadre", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    // POST api/<ParametroBalanceGeneralController>
    [HttpPost]
    public IActionResult CrearParametro([FromHeader(Name = "x-client-id")] int clienteId, [FromBody] ParametroBalanceGeneral parametroBalanceGeneral)
    {
        try
        {
            parametroBalanceGeneral.IdCliente = clienteId;
            return Ok(_parametrosBalanceGeneralService.CrearParametro(parametroBalanceGeneral));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "CrearParametro", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    // PUT api/<ParametroBalanceGeneralController>/5
    [HttpPut("{id}")]
    public IActionResult EditarParametro([FromHeader(Name = "x-client-id")] int clienteId, int id, [FromBody] ParametroBalanceGeneral parametroBalanceGeneral)
    {
        try
        {
            parametroBalanceGeneral.Id = id;
            parametroBalanceGeneral.IdCliente = clienteId;
            return Ok(_parametrosBalanceGeneralService.EditarParametro(parametroBalanceGeneral));
        }
        catch (Exception exception)
        {
            _logService.CrearLog(new Log() { Controlador = CONTROLLER_NAME, Metodo = "EditarParametro", Mensaje = exception.Message });
            return StatusCode(500, "Ha ocurrido un error");
        }
    }

    // DELETE api/<ParametroBalanceGeneralController>/5
    [HttpDelete("{id}")]
    public IActionResult EliminarParametro([FromHeader(Name = "x-client-id")] int clienteId, int id)
    {
        _parametrosBalanceGeneralService.EliminarParametro(clienteId, id);
        return Ok(new MensajeResultado() { Mensaje = "Se ha eliminado parámetro, exitosamente"});
    }
}
