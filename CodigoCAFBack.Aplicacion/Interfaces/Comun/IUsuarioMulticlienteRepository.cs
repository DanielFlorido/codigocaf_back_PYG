

using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Interfaces.Comun
{
    public interface IUsuarioMulticlienteRepository
    {

        List<ResultadoUsuarioMulticliente> ConsultarClientesPorUsuario(int usuarioId);

        ResultadoClienteCorreo ConsultarCorreoCliente(Int32 id);

    }
}
