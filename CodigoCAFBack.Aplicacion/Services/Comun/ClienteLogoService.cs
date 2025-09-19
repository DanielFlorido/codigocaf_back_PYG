
using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Dominio.Comun;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Services.Comun;

public class ClienteLogoService : IClienteLogoService
{

    private readonly IClienteLogoRepository _clienteLogoRepository;

    public ClienteLogoService(IClienteLogoRepository clienteLogoRepository)
    {
        _clienteLogoRepository = clienteLogoRepository;
    }

    public LogoCliente BuscarLogoCliente(LogoClienteRequest logoClienteRequest)
    {
        return _clienteLogoRepository.BuscarLogoCliente(logoClienteRequest);
    }

    public LogoCliente Guardar(LogoCliente logoCliente)
    {
        return _clienteLogoRepository.Guardar(logoCliente);
    }
}
