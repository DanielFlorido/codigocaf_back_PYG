using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Aplicacion.Utilidades;
using CodigoCAFBack.Dominio.Contratos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.ProgramadorPago;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TipoCuentaController : ControllerBase
{

    private readonly ITipoCuentaService _tipoCuentaService;

    public TipoCuentaController(ITipoCuentaService tipoCuentaService)
    {
        _tipoCuentaService = tipoCuentaService;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(_tipoCuentaService.Listar());
    }

    [HttpPost]
    public IActionResult Crear([FromBody] TipoCuenta tipoCuenta)
    {
        return Ok(_tipoCuentaService.Crear(tipoCuenta));
    }

    [HttpPut("{id}")]
    public IActionResult Editar(Int32 id, [FromBody] TipoCuenta tipoCuenta)
    {
        tipoCuenta.Id = id;
        return Ok(_tipoCuentaService.Editar(tipoCuenta));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(Int32 id)
    {
        _tipoCuentaService.Eliminar(id);
        return Ok(new MensajeResultado() { Mensaje = "Se ha eliminado tipo cuenta, exitosamente" });
    }

}
