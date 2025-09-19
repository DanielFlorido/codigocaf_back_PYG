using CodigoCAFBack.Dominio.Comun;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.Comun;

public interface IClienteLogoService
{

    LogoCliente BuscarLogoCliente(LogoClienteRequest logoClienteRequest);

    LogoCliente Guardar(LogoCliente logoCliente);

}
