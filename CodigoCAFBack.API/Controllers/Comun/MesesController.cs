using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.Comun;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MesesController : ControllerBase
{

    private readonly IMesesService _mesesService;

    public MesesController(IMesesService mesesService)
    {
        _mesesService = mesesService;
    }

    [HttpGet("{language}")]
    public IActionResult ListarMeses(string language)
    {
        return Ok(_mesesService.ConsultarMeses(language));
    }

}
