using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IItemProgramadorPagoService
{

    List<ItemProgramadorPagoRequest> ListarItemProgramadorPago(int clienteId);

    ItemProgramadorPagoRequest CrearItemProgramadorPago(ItemProgramadorPagoRequest itemProgramadorPago);

    ItemProgramadorPagoRequest EditarItemProgramadorPago(ItemProgramadorPagoRequest itemProgramadorPago);

    void ElimiarParametro(int id);


}
