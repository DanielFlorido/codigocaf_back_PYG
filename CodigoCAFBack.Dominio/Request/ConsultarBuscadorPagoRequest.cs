
namespace CodigoCAFBack.Dominio.Request;

public class ConsultarBuscadorPagoRequest
{

    public string FechaVencimiento {  get; set; }

    public string CentroCostos {  get; set; }

    public List<string> Identificacion { get; set; }

    public string TipoProveedor { get; set; }

    public string Language { get; set; }

}
