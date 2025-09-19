
namespace CodigoCAFBack.Dominio.Request;

public class BalanceGeneralAnioRequest
{

    public string Year1 { get; set; }
    
    public string Year2 { get; set; } 
    
    public string Language { get; set; }

    public string Type { get; set; }

    public string Group { get; set; }

    public string CodigoPosPre { get; set; }

    public bool IncludeSignature { get; set; }

    public int UserId { get; set; }

    public bool ViewAccountCode { get; set; }

    public string Prefix { get; set; }

}
