using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Aplicacion.Interfaces.Mail;
using CodigoCAFBack.Dominio.Configuracion;
using CodigoCAFBack.Dominio.Request;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CodigoCAFBack.Aplicacion.Services.Mail;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    private readonly IUsuarioMulticlienteService _usuarioMulticlienteService;

    public MailService(IOptions<MailSettings> mailSettings, IUsuarioMulticlienteService usuarioMulticlienteService)
    {
        _mailSettings = mailSettings.Value;
        _usuarioMulticlienteService = usuarioMulticlienteService;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
    {
        await SendEmailAsync(new List<string> { toEmail }, subject, body, null, null, isHtml);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, List<byte[]> attachments, List<string> attachmentNames, bool isHtml = false)
    {
        await SendEmailAsync(new List<string> { toEmail }, subject, body, attachments, attachmentNames, isHtml);
    }

    public async Task SendEmailAsync(List<string> toEmails, string subject, string body, bool isHtml = false)
    {
        await SendEmailAsync(toEmails, subject, body, null, null, isHtml);
    }

    public async Task SendEmailAsync(List<string> toEmails, string subject, string body, List<byte[]> attachments, List<string> attachmentNames, bool isHtml = false)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            foreach (var email in toEmails)
            {
                message.To.Add(new MailAddress(email));
            }

            if (attachments != null)
            {
                for (int i = 0; i < attachments.Count; i++) 
                { 
                    message.Attachments.Add(new Attachment(new MemoryStream(attachments[i]), attachmentNames[i]));
                }
            }

            using var client = new SmtpClient(_mailSettings.SmtpServer, _mailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword),
                EnableSsl = _mailSettings.EnableSsl
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error sending email: {ex.Message}", ex);
        }
    }

    public async Task SendEmailAsync(Int32 idCliente, List<ArchivoRequest> archivos)
    {
        List<byte[]> lista = archivos.Select(x => Convert.FromBase64String(x.Contenido)).ToList();
        SendEmailAsync(_usuarioMulticlienteService.ObtenerCorreoParaCliente(idCliente), "Reporte Solicitado", "Hola, adjunto encontraras el archivo solicitado", lista
            , archivos.Select(x => x.Nombre).ToList(), false);
    }
}