
using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface ITipoCuentaService
{

    List<TipoCuenta> Listar();

    TipoCuenta Crear(TipoCuenta tipoCuenta);

    TipoCuenta Editar(TipoCuenta tipoCuenta);

    void Eliminar(Int32 id);

}
