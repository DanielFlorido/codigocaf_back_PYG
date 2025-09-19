using CodigoCAFBack.Aplicacion.Interfaces.Mail;
using CodigoCAFBack.Dominio.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CodigoCAFBack.API.Controllers.Mail;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MailController : ControllerBase
{
    private readonly IMailService _mailService;

    public MailController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail([FromHeader(Name = "x-client-id")] Int32 idCliente, [FromBody] List<ArchivoRequest> archivoRequests)
    {
        await _mailService.SendEmailAsync(idCliente, archivoRequests);
        return Ok(new { Message = "Email sent successfully" });
    }
}