using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
namespace CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;

public interface IFlujoCajaRepository
{

    List<FlujoCajaAnio> ListarAnio(Int32 idCliente);

    List<LineaFlujoCaja> BuscarEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

    List<LineaFlujoCaja> BuscarSubEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

    List<LineaFlujoCajaTercero> DetalleTercero(Int32 idCliente, FlujoCajaRequest flujoCajaRequest);

    List<TerceroFlujoCaja> BuscarTercero(Int32 idCliente);

}
