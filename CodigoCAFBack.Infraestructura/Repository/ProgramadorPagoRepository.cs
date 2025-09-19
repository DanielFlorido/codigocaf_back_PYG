
using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class ProgramadorPagoRepository : IProgramadorPagoRepository
{

    private readonly CodigoCAFContext _context;

    public ProgramadorPagoRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public List<LineaProgramadorPago> ListarProgramador(Int32 idCliente, ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest)
    {
        return _context.Database.SqlQueryRaw<LineaProgramadorPago>("ProgramadorPagosListar @IdCliente = {0}, @FechaVencimiento = {1}, @CentroCostos = {2}, @Identificacion = {3}, @Lenguaje = {4}, @TipoProveedor = {5}", idCliente,
            consultarBuscadorPagoRequest.FechaVencimiento == null ? DBNull.Value : consultarBuscadorPagoRequest.FechaVencimiento, consultarBuscadorPagoRequest.CentroCostos == null ? DBNull.Value : consultarBuscadorPagoRequest.CentroCostos, 
            consultarBuscadorPagoRequest.Identificacion == null ? DBNull.Value : consultarBuscadorPagoRequest.Identificacion, consultarBuscadorPagoRequest.Language == null ? "es": consultarBuscadorPagoRequest.Language,
            consultarBuscadorPagoRequest.TipoProveedor == null ? DBNull.Value : consultarBuscadorPagoRequest.TipoProveedor).ToList();
    }

    public InfoPagador ConsultarInfoPagador(Int32 idCliente)
    {
        return _context.Database.SqlQueryRaw<InfoPagador>("ConsultarPagador @IdCliente = {0}", idCliente).ToList().FirstOrDefault(new InfoPagador());
    }

}
