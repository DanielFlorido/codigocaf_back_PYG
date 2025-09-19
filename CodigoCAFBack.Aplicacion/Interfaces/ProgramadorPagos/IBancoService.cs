using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IBancoService
{

    List<Banco> Listar();

    Banco Crear(Banco banco);

    Banco Editar(Banco banco);

    void Eliminar(Int32 id);

}
