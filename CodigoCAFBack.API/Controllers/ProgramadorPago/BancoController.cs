using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Contratos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.ProgramadorPago;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BancoController : ControllerBase
{

    private readonly IBancoService _bancoService;

    public BancoController(IBancoService bancoService)
    {
        _bancoService = bancoService;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(_bancoService.Listar());
    }

    [HttpPost]
    public IActionResult Crear([FromBody] Banco banco)
    {
        return Ok(_bancoService.Crear(banco));
    }


    [HttpPut("{id}")]
    public IActionResult Editar(Int32 id, [FromBody] Banco banco)
    {
        banco.Id = id;
        return Ok(_bancoService.Editar(banco));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(Int32 id)
    {
        _bancoService.Eliminar(id);
        return Ok(new MensajeResultado() { Mensaje = "Se ha eliminado parámetro, exitosamente" });
    }


}
