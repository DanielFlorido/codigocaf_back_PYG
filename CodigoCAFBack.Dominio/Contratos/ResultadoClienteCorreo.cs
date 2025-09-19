
namespace CodigoCAFBack.Dominio.Contratos;

public class ResultadoClienteCorreo
{

    public Int32 IdCliente { get; set; }

    public Int16 IdTipoDocumento { get; set; }

    public string NumeroDocumento { get; set; }

    public Int32 DigitoVerificacion { get; set; }

    public string RazonSocial { get; set; }

    public string CorreoElectronico { get; set; }

}
