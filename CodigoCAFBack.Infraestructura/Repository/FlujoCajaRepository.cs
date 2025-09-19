using CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class FlujoCajaRepository : IFlujoCajaRepository
{

    private readonly CodigoCAFContext _context;

    public FlujoCajaRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public List<FlujoCajaAnio> ListarAnio(Int32 idCliente)
    {
        return _context.Database.SqlQueryRaw<FlujoCajaAnio>("ConsultarYearFlujoCajaEncabezado @IdCliente= {0}", idCliente).ToList();
    }

    public List<LineaFlujoCaja> BuscarEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
    {
        return _context.Database.SqlQueryRaw<LineaFlujoCaja>("ConsultarFlujoCajaEncabezado @IdCliente = {0}, @Year = {1}, @Month = {2}, @IdentificacionTercero = {3}, @Language = {4}",
            idCliente, flujoCajaRequest.Year, flujoCajaRequest.Month.HasValue ? flujoCajaRequest.Month.Value : DBNull.Value, flujoCajaRequest.IdentificacionTercero != null ? flujoCajaRequest.IdentificacionTercero : DBNull.Value, flujoCajaRequest.Language)
            .ToList();
    }

    public List<LineaFlujoCaja> BuscarSubEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
    {
        return _context.Database.SqlQueryRaw<LineaFlujoCaja>("ConsultarFlujoCajaDetalle @IdCliente = {0}, @Year = {1}, @Month = {2}, @IdentificacionTercero = {3}, @Language = {4}, @TipoESP = {5}",
           idCliente, flujoCajaRequest.Year, flujoCajaRequest.Month.HasValue ? flujoCajaRequest.Month.Value : DBNull.Value, flujoCajaRequest.IdentificacionTercero != null ? flujoCajaRequest.IdentificacionTercero : DBNull.Value, flujoCajaRequest.Language, flujoCajaRequest.Tipo)
           .ToList();
    }

    public List<LineaFlujoCajaTercero> DetalleTercero(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
    {
        return _context.Database.SqlQueryRaw<LineaFlujoCajaTercero>("ConsultarFlujoCajaClienteDetalle @IdCliente = {0}, @Year = {1}, @Month = {2}, @IdentificacionTercero = {3}, @Language = {4}, @TipoESP = {5}, @POSPRE = {6}",
           idCliente, flujoCajaRequest.Year, flujoCajaRequest.Month.HasValue ? flujoCajaRequest.Month.Value : DBNull.Value, flujoCajaRequest.IdentificacionTercero != null ? flujoCajaRequest.IdentificacionTercero : DBNull.Value, flujoCajaRequest.Language, flujoCajaRequest.Tipo, flujoCajaRequest.CodigoPosPre)
           .ToList();
    }

    public List<TerceroFlujoCaja> BuscarTercero(Int32 idCliente)
    {
        return _context.Database.SqlQueryRaw<TerceroFlujoCaja>("ConsultarTerceroFlujoCaja @IdCliente = {0}", idCliente).ToList();
    }
    

}
