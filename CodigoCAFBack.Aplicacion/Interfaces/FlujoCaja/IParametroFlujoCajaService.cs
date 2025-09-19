

using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IParametroFlujoCajaService
{

    List<ParametroFlujoCaja> ListarParametros(Int32 idCliente);

    ParametroFlujoCajaRequest CrearParametro(ParametroFlujoCajaRequest parametroTercero);

    ParametroFlujoCajaRequest EditarParametro(ParametroFlujoCajaRequest parametroTercero);

    void ElimiarParametro(Int32 idParametro);

}
