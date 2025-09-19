

using CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class ParametroFlujoCajaRepository : IParametroFlujoCajaRepository
{

    private readonly CodigoCAFContext _context;

    public ParametroFlujoCajaRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public List<ParametroFlujoCaja> ListarParametros(Int32 idCliente)
    {
        return _context.Database.SqlQueryRaw<ParametroFlujoCaja>("ParametroFlujoCajaListar @IdCliente = {0}", idCliente).ToList();
    }


    public CrearParametroFlujoCaja CrearParametro(ParametroFlujoCajaRequest parametroFlujoCajaRequest)
    {
        return _context.Database.SqlQueryRaw<CrearParametroFlujoCaja>("ParametroFlujoCajaInsertar @IdentificacionCliente = {0}, @ValorEs = {1}, @ValorEn = {2}, @IdCliente = {3}",
            parametroFlujoCajaRequest.IdentificacionCliente, parametroFlujoCajaRequest.ValorEs, parametroFlujoCajaRequest.ValorEn, parametroFlujoCajaRequest.IdCliente).ToList().First();
    }

    public CrearParametroFlujoCaja EditarParametro(ParametroFlujoCajaRequest parametroFlujoCajaRequest)
    {
        return _context.Database.SqlQueryRaw<CrearParametroFlujoCaja>("ParametroFlujoCajaEditar @Id = {0}, @IdentificacionCliente = {1}, @ValorEs = {2}, @ValorEn = {3}, @IdCliente = {4}",
            parametroFlujoCajaRequest.Id, parametroFlujoCajaRequest.IdentificacionCliente, parametroFlujoCajaRequest.ValorEs, parametroFlujoCajaRequest.ValorEn,
            parametroFlujoCajaRequest.IdCliente).ToList().First();
    }

    public void ElimiarParametro(Int32 idParametro)
    {
        _context.Database.ExecuteSqlRaw("ParametroFlujoCajaEliminar @Id = {0}", idParametro);
    }

}
