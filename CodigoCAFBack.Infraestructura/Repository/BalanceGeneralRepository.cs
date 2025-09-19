using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

/// <summary>
/// Repositorio para la gestión de balances generales. 
/// Proporciona métodos para consultar y manipular datos relacionados con el balance general.
/// </summary>
public class BalanceGeneralRepository : IBalanceGeneralRepository
{

    /// <summary>
    /// Contexto de la base de datos utilizado para acceder a los datos.
    /// </summary>
    private readonly CodigoCAFContext _context;

    /// <summary>
    /// Constructor que inicializa el repositorio con el contexto de la base de datos.
    /// </summary>
    /// <param name="context"></param>
    public BalanceGeneralRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene una lista de años del balance general para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <returns>Una lista de objetos <see cref="BalanceGeneralAnio"/> que representan los años del balance general.</returns>
    public List<BalanceGeneralAnio> ListarAnioBalanceGeneral(Int32 idCliente)
    {
        return _context.Database.SqlQueryRaw<BalanceGeneralAnio>("BalanceGeneralListarYear @IdCliente = {0}", idCliente).ToList();
    }

    /// <summary>
    /// Busca el encabezado del balance general para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralRequest">El objeto <see cref="BalanceGeneralRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns>Una lista de objetos <see cref="LineaBalanceGeneral"/> que representan el encabezado del balance general.</returns>
    public List<LineaBalanceGeneral> BuscarEncabezado(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneral>("ConsultarBalanceGeneralEncabezado @Year = {0}, @Month = {1}, @DateFrom = {2}, @DateTo = {3}, @Language = {4}, @IdCliente = {5}",
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : DBNull.Value, balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : DBNull.Value,
            balanceGeneralRequest.DateFrom != null ? balanceGeneralRequest.DateFrom : DBNull.Value, balanceGeneralRequest.DateTo != null ? balanceGeneralRequest.DateTo : DBNull.Value,
            balanceGeneralRequest.Language, idCliente).ToList();
    }

    /// <summary>
    /// Busca el subencabezado del balance general para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralRequest">El objeto <see cref="BalanceGeneralRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns>Una lista de objetos <see cref="LineaBalanceGeneral"/> que representan el subencabezado del balance general.</returns>
    public List<LineaBalanceGeneral> BuscarSubEncabezado(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneral>("ConsultarBalanceGeneralSubEncabezado @Year = {0}, @Month = {1}, @DateFrom = {2}, @DateTo = {3}, @Language = {4}, @IdCliente = {5}, @Tipo = {6}",
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : DBNull.Value, balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : DBNull.Value,
            balanceGeneralRequest.DateFrom != null ? balanceGeneralRequest.DateFrom : DBNull.Value, balanceGeneralRequest.DateTo != null ? balanceGeneralRequest.DateTo : DBNull.Value,
            balanceGeneralRequest.Language, idCliente, balanceGeneralRequest.Type).ToList();
    }

    public List<GrupoClasificacionCorriente> BuscarClasificacionCorriente(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest, bool esCorriente) {
        return _context.Database.SqlQueryRaw<GrupoClasificacionCorriente>("BalanceGeneralConsultaGrupoCorriente @Year = {0}, @DateFrom = {1}, @DateTo = {2}, @Month = {3}, @IdCliente = {4}, @EsCorriente = {5}",
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : DBNull.Value,
            balanceGeneralRequest.DateFrom != null ? balanceGeneralRequest.DateFrom : DBNull.Value, balanceGeneralRequest.DateTo != null ? balanceGeneralRequest.DateTo : DBNull.Value,
            balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : DBNull.Value, idCliente, esCorriente).ToList();
    }

    /// <summary>
    /// Busca el detalle del balance general para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralRequest">El objeto <see cref="BalanceGeneralRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns>Una lista de objetos <see cref="LineaBalanceGeneral"/> que representan el detalle del balance general.</returns>
    public List<LineaBalanceGeneral> BuscarDetalle(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneral>("ConsultarBalanceGeneralDetalle @Year = {0}, @Month = {1}, @DateFrom = {2}, @DateTo = {3}, @Language = {4}, @IdCliente = {5}, @Grupo = {6}",
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : DBNull.Value, balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : DBNull.Value,
            balanceGeneralRequest.DateFrom != null ? balanceGeneralRequest.DateFrom : DBNull.Value, balanceGeneralRequest.DateTo != null ? balanceGeneralRequest.DateTo : DBNull.Value,
            balanceGeneralRequest.Language, idCliente, balanceGeneralRequest.Grupo).ToList();
    }

    /// <summary>
    /// Busca los terceros del balance general para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralRequest">El objeto <see cref="BalanceGeneralRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns>Una lista de objetos <see cref="LineaBalanceGeneral"/> que representan los terceros del balance general.</returns>
    public List<LineaBalanceGeneral> BuscarTerceros(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneral>("ConsultarBalanceGeneralTercero @Year = {0}, @Month = {1}, @DateFrom = {2}, @DateTo = {3}, @Language = {4}, @IdCliente = {5}, @CodigoPosPre = {6}",
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : DBNull.Value, balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : DBNull.Value,
            balanceGeneralRequest.DateFrom != null ? balanceGeneralRequest.DateFrom : DBNull.Value, balanceGeneralRequest.DateTo != null ? balanceGeneralRequest.DateTo : DBNull.Value,
            balanceGeneralRequest.Language, idCliente, balanceGeneralRequest.CodigoPosPre).ToList();
    }

    /// <summary>
    /// Busca el encabezado del balance general agrupado por año para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralAnioRequest">El objeto <see cref="BalanceGeneralAnioRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns >Una lista de objetos <see cref="LineaBalanceGeneralAnio"/> que representan el encabezado del balance general agrupado por año.</returns>
    public List<LineaBalanceGeneralAnio> BuscarEncabezadoAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneralAnio>("BalanceGeneralGrupoConsultar @Year1 = {0}, @Year2 = {1}, @IdCliente = {2}, @Language = {3}",
            balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, idCliente, balanceGeneralAnioRequest.Language).ToList();
    }

    /// <summary>
    /// Busca el subencabezado del balance general agrupado por año para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralAnioRequest">El objeto <see cref="BalanceGeneralAnioRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns>Una lista de objetos <see cref="LineaBalanceGeneralAnio"/> que representan el subencabezado del balance general agrupado por año.</returns>
    public List<LineaBalanceGeneralAnio> BuscarSubEncabezadoAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneralAnio>("BalanceGeneralPorTipoGrupoConsultar @Year1 = {0}, @Year2 = {1}, @IdCliente = {2}, @Language = {3}, @Type = {4}",
            balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, idCliente, balanceGeneralAnioRequest.Language, balanceGeneralAnioRequest.Type).ToList();
    }

    public List<GrupoClasificacionCorriente> BuscarGrupoClasificacionCorriente(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest, bool esCorriente)
    {
        return _context.Database.SqlQueryRaw<GrupoClasificacionCorriente>("BalanceGeneralAgrupadoConsultaGrupoCorriente @Year1 = {0}, @Year2 = {1}, @IdCliente = {2}, @EsCorriente = {3}",
            balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, idCliente, esCorriente).ToList();
    }

    /// <summary>
    /// Busca el detalle del balance general agrupado por año para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralAnioRequest">El objeto <see cref="BalanceGeneralAnioRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns>Una lista de objetos <see cref="LineaBalanceGeneralAnio"/> que representan el detalle del balance general agrupado por año.</returns>
    public List<LineaBalanceGeneralAnio> BuscarDetalleAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneralAnio>("BalanceGeneralDetelleGrupoConsultar @Year1 = {0}, @Year2 = {1}, @IdCliente = {2}, @Language = {3}, @Grupo = {4}",
            balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, idCliente, balanceGeneralAnioRequest.Language, balanceGeneralAnioRequest.Group).ToList();
    }

    /// <summary>
    /// Busca los detalles de los terceros del balance general agrupado por año para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="balanceGeneralAnioRequest">El objeto <see cref="BalanceGeneralAnioRequest"/> que contiene los parámetros de búsqueda.</param>
    /// <returns>Una lista de objetos <see cref="LineaBalanceGeneralAnioDet"/> que representan los detalles de los terceros del balance general agrupado por año.</returns>
    public List<LineaBalanceGeneralAnioDet> BuscarTerceroAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneralAnioDet>("BalanceGeneralPorCodigoPosPreGrupoConsultar @Year1 = {0}, @Year2 = {1}, @IdCliente = {2}, @Language = {3}, @CodigoPosPre = {4}",
            balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, idCliente, balanceGeneralAnioRequest.Language, balanceGeneralAnioRequest.CodigoPosPre).ToList();
    }

    /// <summary>
    /// Guarda una nota en el balance general para un cliente específico.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="notaBalanceGeneral">El objeto <see cref="NotaBalanceGeneralRequest"/> que contiene los datos de la nota a guardar.</param>
    /// <returns></returns>
    public Int32 GuardarNota(Int32 idCliente, NotaBalanceGeneralRequest notaBalanceGeneral)
    {
        return _context.Database.SqlQueryRaw<Crear>("BalanceGeneralNotaInsertar @CodigoCuentaConble = {0}, @Titulo = {1}, @Valor = {2}, @Nota = {3}, @Anio = {4}, @Mes = {5}, @IdCliente = {6}", 
            notaBalanceGeneral.Account, notaBalanceGeneral.NoteHead, notaBalanceGeneral.NoteValue, notaBalanceGeneral.NoteContent, notaBalanceGeneral.Year, notaBalanceGeneral.Month, idCliente).ToList().First().ID;  
    }

}
