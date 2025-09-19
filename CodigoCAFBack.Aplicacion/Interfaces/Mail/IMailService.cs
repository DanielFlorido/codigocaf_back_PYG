using CodigoCAFBack.Dominio.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodigoCAFBack.Aplicacion.Interfaces.Mail;

public interface IMailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false);
    Task SendEmailAsync(string toEmail, string subject, string body, List<byte[]> attachments, List<string> attachmentNames, bool isHtml = false);
    Task SendEmailAsync(List<string> toEmails, string subject, string body, bool isHtml = false);
    Task SendEmailAsync(List<string> toEmails, string subject, string body, List<byte[]> attachments, List<string> attachmentNames, bool isHtml = false);
    Task SendEmailAsync(Int32 idCliente, List<ArchivoRequest> archivos);
}