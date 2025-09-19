using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Dominio.Contratos;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura.Repository;

public class UsuarioMulticlienteRepository : IUsuarioMulticlienteRepository
{

    private readonly CodigoCAFContext _context;

    public UsuarioMulticlienteRepository(CodigoCAFContext context)
    {
        _context = context;
    }

    public List<ResultadoUsuarioMulticliente> ConsultarClientesPorUsuario(int usuarioId)
    {
        return _context.Database.SqlQueryRaw<ResultadoUsuarioMulticliente>("MULTICLIENTESCONSULTAR @IDUSUARIO = {0}", usuarioId).ToList();
    }

    public ResultadoClienteCorreo ConsultarCorreoCliente(Int32 clienteId)
    {
        return _context.Database.SqlQueryRaw<ResultadoClienteCorreo>("ClientesConsultarCorreo @IdCliente = {0}", clienteId).ToList().FirstOrDefault(new ResultadoClienteCorreo() { IdCliente = clienteId});
    }

}
