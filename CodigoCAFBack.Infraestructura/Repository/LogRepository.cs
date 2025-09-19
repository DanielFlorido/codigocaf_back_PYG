
using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Dominio.Log;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class LogRepository : ILogRepository
{

    private readonly CodigoCAFContext _context;

    public LogRepository(CodigoCAFContext context)
    {
        _context = context;
    } 

    public void CrearLog(Log log)
    {
        _context.Database.ExecuteSqlRaw("LogInserta @Controlador = {0}, @Metodo = {1}, @Mensaje = {2}", log.Controlador, log.Metodo, log.Mensaje);
    }
}
