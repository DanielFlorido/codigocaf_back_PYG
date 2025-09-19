using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Services.ProgramadorPago;

public class BancoService : IBancoService
{

    private readonly IBancoRepository _bancoRepository;

    public BancoService(IBancoRepository bancoRepository)
    {
        _bancoRepository = bancoRepository;
    }

    public List<Banco> Listar()
    {
        return _bancoRepository.Listar();
    }

    public Banco Crear(Banco banco)
    {
        banco.Id = _bancoRepository.Crear(banco);
        return banco;
    }

    public Banco Editar(Banco banco)
    {
        _bancoRepository.Editar(banco);
        return banco;
    }

    public void Eliminar(Int32 id)
    {
        _bancoRepository.Eliminar(id);
    }
}
