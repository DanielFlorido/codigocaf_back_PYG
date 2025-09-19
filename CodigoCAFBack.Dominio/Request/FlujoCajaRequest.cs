

namespace CodigoCAFBack.Dominio.Request;

public class FlujoCajaRequest
{

    public Int32 Year { get; set; }
    
    public string IdentificacionTercero { get; set; }
    
    public string Language { get; set; }
    
    public Nullable<Int32> Month { get; set; }

    public string Tipo { get; set; }

    public string CodigoPosPre { get; set; }

    public List<string> Meses { get; set; }

    public bool IncludeSignature { get; set; }

    public int UserId { get; set; }

}
