using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Dominio.Comun;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.Comun;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ClienteLogoController : ControllerBase
{

    private readonly IClienteLogoService _clienteLogoService;

    public ClienteLogoController(IClienteLogoService clienteLogoService)
    {
        _clienteLogoService = clienteLogoService;
    }

    [HttpPost]
    public IActionResult BuscarLogoCliente(LogoClienteRequest logoClienteRequest)
    {
        return Ok(_clienteLogoService.BuscarLogoCliente(logoClienteRequest));
    }

    [HttpPost("Guardar")]
    public IActionResult GuardarLogoCliente(LogoCliente logoCliente)
    {
        return Ok(_clienteLogoService.Guardar(logoCliente));
    }


}
