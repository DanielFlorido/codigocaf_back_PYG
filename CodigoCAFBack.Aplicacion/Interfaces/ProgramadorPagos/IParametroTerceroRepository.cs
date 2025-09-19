

using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IParametroTerceroRepository
{

    List<ParametroTercero> ListarParametros(Int32 idCliente);

    List<TipoProveedor> ListarTipoProveedor(Int32 idCliente, string language);

    CrearTercero CrearParametro(ParametroTerceroRequest parametroTercero);

    CrearTercero EditarParametro(ParametroTerceroRequest parametroTercero);

    void ElimiarParametro(Int32 idParametro);

}
