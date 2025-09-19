
using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IBancoRepository
{

    List<Banco> Listar();

    Int32 Crear(Banco banco);

    void Editar(Banco banco);

    void Eliminar(Int32 id); 

}
