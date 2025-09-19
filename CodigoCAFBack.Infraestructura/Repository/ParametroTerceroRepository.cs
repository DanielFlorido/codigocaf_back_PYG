

using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class ParametroTerceroRepository : IParametroTerceroRepository
{

    private readonly CodigoCAFContext _context;

    public ParametroTerceroRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public List<ParametroTercero> ListarParametros(Int32 idCliente)
    {
        return _context.Database.SqlQueryRaw<ParametroTercero>("ParametroTercerosListar @IdCliente = {0}", idCliente).ToList();
    }

    public List<TipoProveedor> ListarTipoProveedor(Int32 idCliente, string language)
    {
        return _context.Database.SqlQueryRaw<TipoProveedor>("ConsultarTipoProveedor @IdCliente = {0}, @Language = {1}", idCliente, language).ToList();
    }

    public CrearTercero CrearParametro(ParametroTerceroRequest parametroTercero)
    {
        return _context.Database.SqlQueryRaw<CrearTercero>("ParametroTercerosInsertar @Identificacion = {0}, @Nombre = {1}, @TipoProveedorEs = {2}, @TipoProveedorEn = {3}, " +
            "@CodigoACH = {4}, @TipoCuentaBancaria = {5}, @CuentaBancaria = {6}, @TipoDocumento = {7}, @CorreoElectronico = {8}, @IdCliente = {9}", parametroTercero.Identificacion,
            parametroTercero.Nombre, parametroTercero.TipoProveedorEs, parametroTercero.TipoProveedorEn, parametroTercero.CodigoACH, parametroTercero.TipoCuentaBancaria,
            parametroTercero.CuentaBancaria, parametroTercero.TipoDocumento, parametroTercero.CorreoElectronico, parametroTercero.IdCliente).ToList().First();
    }

    public CrearTercero EditarParametro(ParametroTerceroRequest parametroTercero)
    {
        return _context.Database.SqlQueryRaw<CrearTercero>("ParametroTercerosEditar @Id = {0}, @Identificacion = {1}, @Nombre = {2}, @TipoProveedorEs = {3}, @TipoProveedorEn = {4}, " +
            "@CodigoACH = {5}, @TipoCuentaBancaria = {6}, @CuentaBancaria = {7}, @TipoDocumento = {8}, @CorreoElectronico = {9}, @IdCliente = {10}",
            parametroTercero.Id, parametroTercero.Identificacion,
            parametroTercero.Nombre, parametroTercero.TipoProveedorEs, parametroTercero.TipoProveedorEn, parametroTercero.CodigoACH, parametroTercero.TipoCuentaBancaria,
            parametroTercero.CuentaBancaria, parametroTercero.TipoDocumento, parametroTercero.CorreoElectronico, parametroTercero.IdCliente).ToList().First();
    }

    public void ElimiarParametro(Int32 idParametro)
    {
        _context.Database.ExecuteSqlRaw("ParametroTercerosEliminar @Id = {0}", idParametro);
    }

}
