
using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Services.ProgramadorPago;

public class TipoCuentaService : ITipoCuentaService
{

    private readonly ITipoCuentaRepository _tipoCuentaRepository;

    public TipoCuentaService(ITipoCuentaRepository tipoCuentaRepository)
    {
        _tipoCuentaRepository = tipoCuentaRepository;
    }

    public List<TipoCuenta> Listar()
    {
        return _tipoCuentaRepository.Listar();
    }

    public TipoCuenta Crear(TipoCuenta tipoCuenta)
    {
        Int32 id = _tipoCuentaRepository.Crear(tipoCuenta);
        tipoCuenta.Id = id;
        return tipoCuenta;
    }

    public TipoCuenta Editar(TipoCuenta tipoCuenta)
    {
        _tipoCuentaRepository.Editar(tipoCuenta);

        return tipoCuenta;
    }

    public void Eliminar(Int32 id)
    {
        _tipoCuentaRepository.Eliminar(id);
    }

}
