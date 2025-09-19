
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;

public interface IFlujoCajaService
{

    List<FlujoCajaAnio> ListarAnio(Int32 idCliente);

    FlujoCajaResultado ConsultarEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

    FlujoCajaResultado ConsultarSubEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

    FlujoCajaResultado DetalleTercero(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

    List<TerceroFlujoCaja> BuscarTercero(Int32 idCliente);

    byte[] GenerarExcel(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

    void EnviarExcel(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

}
