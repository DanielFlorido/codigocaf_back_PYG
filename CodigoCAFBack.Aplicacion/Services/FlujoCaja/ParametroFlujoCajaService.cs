using CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;
using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Services.FlujoCaja;

public class ParametroFlujoCajaService : IParametroFlujoCajaService
{

    private readonly IParametroFlujoCajaRepository _parametroFlujoCajaRepository;

    public ParametroFlujoCajaService(IParametroFlujoCajaRepository parametroFlujoCajaRepository)
    {
        _parametroFlujoCajaRepository = parametroFlujoCajaRepository;
    }

    public ParametroFlujoCajaRequest CrearParametro(ParametroFlujoCajaRequest parametroTercero)
    {
        CrearParametroFlujoCaja crearParametro = _parametroFlujoCajaRepository.CrearParametro(parametroTercero);
        parametroTercero.Id = crearParametro.Id;
        return parametroTercero;
    }

    public ParametroFlujoCajaRequest EditarParametro(ParametroFlujoCajaRequest parametroTercero)
    {
        _parametroFlujoCajaRepository.EditarParametro(parametroTercero);
        return parametroTercero;
    }

    public void ElimiarParametro(int idParametro)
    {
        _parametroFlujoCajaRepository.ElimiarParametro(idParametro);
    }

    public List<ParametroFlujoCaja> ListarParametros(int idCliente)
    {
        return _parametroFlujoCajaRepository.ListarParametros(idCliente);
    }
}
