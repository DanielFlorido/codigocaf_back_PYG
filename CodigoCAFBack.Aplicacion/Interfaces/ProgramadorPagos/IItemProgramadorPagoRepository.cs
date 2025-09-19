
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IItemProgramadorPagoRepository
{

    List<ItemProgramadorPagoRequest> Listar(int idCliente);

    Crear Crear(ItemProgramadorPagoRequest itemProgramadorPagoRequest);

    Crear Editar(ItemProgramadorPagoRequest itemProgramadorPagoRequest);

    void Eliminar(int idItemProgramadorPago);

}
