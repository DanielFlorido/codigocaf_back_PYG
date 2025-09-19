using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Dominio.Comun;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class ClienteLogoRepository : IClienteLogoRepository
{

    private readonly CodigoCAFContext _context;

    public ClienteLogoRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public LogoCliente BuscarLogoCliente(LogoClienteRequest logoClienteRequest)
    {
        return _context.Database
            .SqlQueryRaw<LogoCliente>("ConsultarLogosPorCliente @IdCliente = {0}, @IdUsuario = {1}", logoClienteRequest.IdCliente, logoClienteRequest.IdUsuario)
            .ToList().FirstOrDefault(new LogoCliente() { IdCliente = logoClienteRequest.IdCliente, IdUsuario= logoClienteRequest.IdUsuario});
    }

    public LogoCliente Guardar(LogoCliente logoCliente)
    {
        return _context.Database
            .SqlQueryRaw<LogoCliente>("GuardarLogosPorCliente @IdCliente = {0}, @IdUsuario = {1}, @LogoPrincipal = {2}, @LogoAuxiliar = {3}," +
                "@FirmaContador = {4}, @FirmaRevisor = {5}, @FirmaGerente = {6}", logoCliente.IdCliente, logoCliente.IdUsuario, logoCliente.LogoPrincipal == null ? DBNull.Value : logoCliente.LogoPrincipal,
                logoCliente.LogoAuxiliar == null ? DBNull.Value : logoCliente.LogoAuxiliar, logoCliente.FirmaContador == null ? DBNull.Value : logoCliente.FirmaContador,
                logoCliente.FirmaRevisor == null ? DBNull.Value : logoCliente.FirmaRevisor, logoCliente.FirmaGerente == null ? DBNull.Value : logoCliente.FirmaGerente)
            .ToList().FirstOrDefault(new LogoCliente { IdCliente = logoCliente.IdCliente, IdUsuario = logoCliente.IdUsuario});
    }

}
