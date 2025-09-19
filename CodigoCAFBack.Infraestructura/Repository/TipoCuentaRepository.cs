
using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class TipoCuentaRepository : ITipoCuentaRepository
{

    private readonly CodigoCAFContext _context;

    public TipoCuentaRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public List<TipoCuenta> Listar()
    {
        return _context.Database.SqlQueryRaw<TipoCuenta>("TipoCuentasListar").ToList();
    }

    public Int32 Crear(TipoCuenta tipoCuenta)
    {
        return _context.Database.SqlQueryRaw<Crear>("TipoCuentasInsertar @Nombre = {0}, @Codigo = {1}", tipoCuenta.Nombre, tipoCuenta.Codigo).ToList().First().ID;
    }

    public void Editar(TipoCuenta tipoCuenta)
    {
        _context.Database.ExecuteSqlRaw("TipoCuentasEditar @Id = {0}, @Nombre = {1}, @Codigo = {2}", tipoCuenta.Id, tipoCuenta.Nombre, tipoCuenta.Codigo);
    }

    public void Eliminar(int id)
    {
        _context.Database.ExecuteSqlRaw("TipoCuentasEliminar @Id = {0}", id);
    }

}
