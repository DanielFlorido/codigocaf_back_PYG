using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class ItemProgramadorPagoRepository : IItemProgramadorPagoRepository
{

    private readonly CodigoCAFContext _context;

    public ItemProgramadorPagoRepository(CodigoCAFContext context)
    {
        _context = context;
    }
    public List<ItemProgramadorPagoRequest> Listar(int idCliente)
    {
        return _context.Database.SqlQueryRaw<ItemProgramadorPagoRequest>("ProgramadorPagoItemListar @IdCliente = {0}", idCliente).ToList();
    }

    public Crear Crear(ItemProgramadorPagoRequest itemProgramadorPagoRequest)
    {
        return _context.Database.SqlQueryRaw<Crear>("ProgramadorPagoItemInsertar @ItemEs = {0}, @ItemEn = {1}, @Cuentas = {2}, @IdCliente = {3}",
            itemProgramadorPagoRequest.ItemEs, itemProgramadorPagoRequest.ItemEn, itemProgramadorPagoRequest.Cuentas, itemProgramadorPagoRequest.IdCliente).ToList().FirstOrDefault();
    }

    public Crear Editar(ItemProgramadorPagoRequest itemProgramadorPagoRequest)
    {
        return _context.Database.SqlQueryRaw<Crear>("ProgramadorPagoItemEditar @Id = {0}, @ItemEs = {1}, @ItemEn = {2}, @Cuentas = {3}, @IdCliente = {4}",
            itemProgramadorPagoRequest.Id, itemProgramadorPagoRequest.ItemEs, itemProgramadorPagoRequest.ItemEn, itemProgramadorPagoRequest.Cuentas, itemProgramadorPagoRequest.IdCliente).ToList().FirstOrDefault();
    }

    public void Eliminar(int idItemProgramadorPago)
    {
        _context.Database.ExecuteSqlRaw("ProgramadorPagoItemEliminar @Id = {0}", idItemProgramadorPago);
    }

}
