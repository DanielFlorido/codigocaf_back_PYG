using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Interfaces.Comun
{
    public interface IUsuarioMulticlienteService
    {

        List<ResultadoUsuarioMulticliente> ConsultarClientesPorUsuario(int usuarioId);

        string ObtenerCorreoParaCliente(Int32 ClienteId);

    }
}
