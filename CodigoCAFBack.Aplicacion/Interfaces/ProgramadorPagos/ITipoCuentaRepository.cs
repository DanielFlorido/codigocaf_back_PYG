
using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface ITipoCuentaRepository
{

    List<TipoCuenta> Listar();

    Int32 Crear(TipoCuenta tipoCuenta);

    void Editar(TipoCuenta tipoCuenta);

    void Eliminar(Int32 id);

}
