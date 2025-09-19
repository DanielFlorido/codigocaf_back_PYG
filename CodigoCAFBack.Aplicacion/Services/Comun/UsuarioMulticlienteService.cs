using CodigoCAFBack.Aplicacion.Excepciones;
using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Dominio.Contratos;

namespace CodigoCAFBack.Aplicacion.Services.Comun
{
    public class UsuarioMulticlienteService : IUsuarioMulticlienteService
    {

        private readonly IUsuarioMulticlienteRepository _usuarioMulticlienteRepository;

        public UsuarioMulticlienteService(IUsuarioMulticlienteRepository usuarioMulticlienteRepository)
        {
            _usuarioMulticlienteRepository = usuarioMulticlienteRepository;
        }

        public List<ResultadoUsuarioMulticliente> ConsultarClientesPorUsuario(int usuarioId)
        {
            return _usuarioMulticlienteRepository.ConsultarClientesPorUsuario(usuarioId);
        }

        public string ObtenerCorreoParaCliente(Int32 ClienteId)
        {
            ResultadoClienteCorreo resultadoClienteCorreo = _usuarioMulticlienteRepository.ConsultarCorreoCliente(ClienteId);
            if(resultadoClienteCorreo == null)
            {
                throw new CodigoCAFExcepcion("No se encuentra el cliente " + ClienteId);
            }
            if (string.IsNullOrEmpty(resultadoClienteCorreo.CorreoElectronico))
            {
                throw new CodigoCAFExcepcion("No se encuentra correo eléctronico configurado para el cliente " + ClienteId);
            }
            return resultadoClienteCorreo.CorreoElectronico;
        }
    }
}
