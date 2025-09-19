
namespace CodigoCAFBack.Dominio.Contratos;

public class ResultadoUsuarioMulticliente
{

    public Int32 Id { get; set; }

    public Int32 IdUsuario { get; set; }

    public Int32 IDCLIENTE { get; set; }

    public bool ESTADO { get; set; }

    public string RazonSocial { get; set; }

    public string NumeroDocumento { get; set; }

}
