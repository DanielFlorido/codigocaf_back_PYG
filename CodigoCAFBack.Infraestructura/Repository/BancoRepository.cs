
using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

///<summary>
/// Repositorio para gestionar las operaciones relacionadas con la entidad Banco.
/// Proporciona métodos para listar, crear, editar y eliminar bancos en la base de datos.
/// </summary>
public class BancoRepository : IBancoRepository
{

    /// <summary>
    /// Contexto de la base de datos utilizado para acceder a los datos.
    /// </summary>
    private readonly CodigoCAFContext _context;

    /// <summary>
    /// Constructor que inicializa el repositorio con el contexto de la base de datos.
    /// </summary>
    /// <param name="context">El contexto de la base de datos.</param>
    public BancoRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos los bancos en la base de datos.
    /// </summary>
    /// <returns>Una lista de objetos Banco.</returns>
    public List<Banco> Listar()
    {
        return _context.Database.SqlQueryRaw<Banco>("BancosListar").ToList();
    }

    /// <summary>
    /// Crea un nuevo banco en la base de datos.
    /// </summary>
    /// <param name="banco">Objeto Banco que contiene la información del banco a crear.</param>
    /// <returns>El id del objeto creado.</returns>
    public Int32 Crear(Banco banco)
    {
        return _context.Database.SqlQueryRaw<Crear>("BancosInsertar @Nombre = {0}, @CodigoACH = {1}", banco.Nombre, banco.CodigoACH).ToList().First().ID;
    }

    /// <summary>
    /// Edita un banco existente en la base de datos.
    /// </summary>
    /// <paramref name="banco"/>Objeto Banco que contiene la información del banco a editar.</param>
    public void Editar(Banco banco)
    {
        _context.Database.ExecuteSqlRaw("BancosEditar @Id = {0}, @Nombre = {1}, @CodigoACH = {2}", banco.Id, banco.Nombre, banco.CodigoACH);
    }

    /// <summary>
    /// Elimina un banco de la base de datos.
    ///</summary>
    ///<param name="id">El identificador del banco a eliminar.</param>
    public void Eliminar(Int32 id)
    {
        _context.Database.ExecuteSqlRaw("BancosEliminar @Id = {0}", id);
    }

}
