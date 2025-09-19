using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Services.ProgramadorPago;

public class ItemProgramadorPagoService : IItemProgramadorPagoService
{

    private readonly IItemProgramadorPagoRepository _lineaProgramadorPagoRepository;

    public ItemProgramadorPagoService(IItemProgramadorPagoRepository lineaProgramadorPagoRepository)
    {
        _lineaProgramadorPagoRepository = lineaProgramadorPagoRepository;
    }

    public ItemProgramadorPagoRequest CrearItemProgramadorPago(ItemProgramadorPagoRequest itemProgramadorPago)
    {
        Crear resultado = _lineaProgramadorPagoRepository.Crear(itemProgramadorPago);
        itemProgramadorPago.Id = resultado.ID;
        return itemProgramadorPago;
    }

    public ItemProgramadorPagoRequest EditarItemProgramadorPago(ItemProgramadorPagoRequest itemProgramadorPago)
    {
        _lineaProgramadorPagoRepository.Editar(itemProgramadorPago);
        return itemProgramadorPago;
    }

    public void ElimiarParametro(int id)
    {
        _lineaProgramadorPagoRepository.Eliminar(id);
    }

    public List<ItemProgramadorPagoRequest> ListarItemProgramadorPago(int clienteId)
    {
        return _lineaProgramadorPagoRepository.Listar(clienteId);
    }
}
