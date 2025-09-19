
using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Dominio.Log;

namespace CodigoCAFBack.Infraestructura.Repository;

public class LogService : ILogService
{

    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository iLogRepository)
    {
        _logRepository = iLogRepository;
    } 

    public void CrearLog(Log log)
    {
        _logRepository.CrearLog(log);
    }
}
