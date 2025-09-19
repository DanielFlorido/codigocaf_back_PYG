using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;

public interface IParametroFlujoCajaRepository
{

    List<ParametroFlujoCaja> ListarParametros(Int32 idCliente);

    CrearParametroFlujoCaja CrearParametro(ParametroFlujoCajaRequest parametroFlujoCajaRequest);

    CrearParametroFlujoCaja EditarParametro(ParametroFlujoCajaRequest parametroFlujoCajaRequest);

    void ElimiarParametro(Int32 idParametro);

}
