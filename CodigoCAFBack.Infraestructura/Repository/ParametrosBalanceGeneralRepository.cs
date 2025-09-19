using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class ParametrosBalanceGeneralRepository : IParametrosBalanceGeneralRepository
{

    private readonly CodigoCAFContext _context;

    public ParametrosBalanceGeneralRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public List<ParametroBalanceGeneral> ListarParametros(Int32 clienteId)
    {
        return _context.Database.SqlQueryRaw<ParametroBalanceGeneral>("ParametrosBalanceGeneralListar @IdCliente = {0}", clienteId).ToList();
    }

    public List<ParametroBalanceGeneral> ListarParametrosPorIdPadre(Int32 clienteId, Nullable<Int32> idPadre)
    {
        return _context.Database.SqlQueryRaw<ParametroBalanceGeneral>("ParametrosBalanceGeneralBuscarPorIDPadre @IdCliente = {0}, @IdPadre = {1}", clienteId, idPadre.HasValue ? idPadre.Value : DBNull.Value).ToList();
    }

    public Int32 CrearParametro(ParametroBalanceGeneral parametroBalanceGeneral)
    {
        Console.WriteLine(parametroBalanceGeneral.ToString());
        return _context.Database.SqlQueryRaw<Crear>("ParametrosBalanceGeneralInsertar @Codigo = {0}, @NombreEs = {1}, @NombreEn = {2}, @PosPreEs = {3}, @PosPreEn = {4}, " +
            "@AmortizacionIntereses = {5}, @IdPadre = {6}, @IdCliente = {7}, @EsCorriente = {8}, @FlujoCaja = {9}", parametroBalanceGeneral.Codigo, parametroBalanceGeneral.NombreEs, parametroBalanceGeneral.NombreEn,
            parametroBalanceGeneral.PosPreEs, parametroBalanceGeneral.PosPreEn, parametroBalanceGeneral.AmortizacionIntereses, parametroBalanceGeneral.IdPadre.HasValue ? parametroBalanceGeneral.IdPadre.Value : DBNull.Value, 
            parametroBalanceGeneral.IdCliente, parametroBalanceGeneral.EsCorriente, parametroBalanceGeneral.FlujoCaja).ToList().First().ID;
    }

    public void EditarParametro(ParametroBalanceGeneral parametroBalanceGeneral)
    {
        _context.Database.ExecuteSqlRaw("ParametrosBalanceGeneralActualizar @Id = {0}, @Codigo = {1}, @NombreEs = {2}, @NombreEn = {3}, @PosPreEs = {4}, @PosPreEn = {5}, " +
        "@AmortizacionIntereses = {6}, @IdPadre = {7}, @IdCliente = {8}, @EsCorriente = {9}, @FlujoCaja = {10}", parametroBalanceGeneral.Id, parametroBalanceGeneral.Codigo, parametroBalanceGeneral.NombreEs, parametroBalanceGeneral.NombreEn,
        parametroBalanceGeneral.PosPreEs, parametroBalanceGeneral.PosPreEn, parametroBalanceGeneral.AmortizacionIntereses, parametroBalanceGeneral.IdPadre.HasValue ? parametroBalanceGeneral.IdPadre.Value : null, 
        parametroBalanceGeneral.IdCliente, parametroBalanceGeneral.EsCorriente, parametroBalanceGeneral.FlujoCaja);
    }

    public void EliminarParametro(Int32 clienteId, Int32 id)
    {
        _context.Database.ExecuteSqlRaw("ParametrosBalanceGeneralEliminar @IdCliente = {0}, @Id = {1}", clienteId, id);
    }
}
