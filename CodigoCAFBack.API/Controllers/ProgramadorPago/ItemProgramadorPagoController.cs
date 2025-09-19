using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Aplicacion.Services.FlujoCaja;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.ProgramadorPago;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ItemProgramadorPagoController : ControllerBase
{
    
    private readonly IItemProgramadorPagoService _lineaProgramadorPagoService;

    public ItemProgramadorPagoController(IItemProgramadorPagoService lineaProgramadorPagoService)
    {
        _lineaProgramadorPagoService = lineaProgramadorPagoService;
    }

    [HttpGet]
    public IActionResult GetParametro([FromHeader(Name = "x-client-id")] int clienteId)
    {
        return Ok(_lineaProgramadorPagoService.ListarItemProgramadorPago(clienteId));
    }

    [HttpPost]
    public IActionResult Crear([FromHeader(Name = "x-client-id")] int idCliente, [FromBody] ItemProgramadorPagoRequest itemProgramadorPago)
    {
        itemProgramadorPago.IdCliente = idCliente;
        return Ok(_lineaProgramadorPagoService.CrearItemProgramadorPago(itemProgramadorPago));
    }


    [HttpPut("{id}")]
    public IActionResult Editar(Int32 id, [FromHeader(Name = "x-client-id")] int idCliente, [FromBody] ItemProgramadorPagoRequest itemProgramadorPago)
    {
        itemProgramadorPago.Id = id;
        itemProgramadorPago.IdCliente = idCliente;
        return Ok(_lineaProgramadorPagoService.EditarItemProgramadorPago(itemProgramadorPago));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(Int32 id)
    {
        _lineaProgramadorPagoService.ElimiarParametro(id);
        return Ok(new MensajeResultado() { Mensaje = "Se ha eliminado el ítem, exitosamente" });
    }

}
