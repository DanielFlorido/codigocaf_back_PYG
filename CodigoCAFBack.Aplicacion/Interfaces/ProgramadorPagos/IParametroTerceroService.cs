

using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IParametroTerceroService
{

    List<ParametroTercero> ListarParametros(Int32 idCliente);

    List<TipoProveedor> ListarTipoProveedor(Int32 idCliente, string language);

    ParametroTerceroRequest CrearParametro(ParametroTerceroRequest parametroTercero);

    ParametroTerceroRequest EditarParametro(ParametroTerceroRequest parametroTercero);

    void ElimiarParametro(Int32 idParametro);

}
