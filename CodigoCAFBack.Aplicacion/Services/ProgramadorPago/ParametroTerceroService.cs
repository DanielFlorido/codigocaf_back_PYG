

using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Services.ProgramadorPago;

public class ParametroTerceroService : IParametroTerceroService
{

    private readonly IParametroTerceroRepository _terceroRepository;

    public ParametroTerceroService(IParametroTerceroRepository terceroRepository)
    {
        _terceroRepository = terceroRepository;
    }

    public List<ParametroTercero> ListarParametros(Int32 idCliente)
    {
        return _terceroRepository.ListarParametros(idCliente);
    }

    public List<TipoProveedor> ListarTipoProveedor(Int32 idCliente, string language)
    {
        return _terceroRepository.ListarTipoProveedor(idCliente, language);
    }

    public ParametroTerceroRequest CrearParametro(ParametroTerceroRequest parametroTercero)
    {
        CrearTercero crearTercero = _terceroRepository.CrearParametro(parametroTercero);
        parametroTercero.Id = crearTercero.ID;
        parametroTercero.BancoPagador = crearTercero.BancoPagador;
        parametroTercero.Banco = crearTercero.Banco;
        return parametroTercero;
    }

    public ParametroTerceroRequest EditarParametro(ParametroTerceroRequest parametroTercero)
    {
        CrearTercero crearTercero = _terceroRepository.EditarParametro(parametroTercero);
        parametroTercero.BancoPagador = crearTercero.BancoPagador;
        parametroTercero.Banco = crearTercero.Banco;
        return parametroTercero;
    }

    public void ElimiarParametro(Int32 idParametro)
    {
        _terceroRepository.ElimiarParametro(idParametro);
    }

}
