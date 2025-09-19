
namespace CodigoCAFBack.Dominio.Request;

public class BalanceGeneralRequest
{
    public Nullable<Int32> Year { get; set; }
    
    public string DateFrom { get; set; }

    public string DateTo { get; set; }

    public string Language { get; set; }

    public Nullable<Int32> Month { get; set; }

    public string Type { get; set; }

    public string Grupo { get; set; }

    public string CodigoPosPre { get; set; }

    public List<String> etiquetas { get; set; }

    public bool IncludeSignature { get; set; }
    public int UserId { get; set; }

    public bool ViewAccountCode { get; set; }

    public string Prefix { get; set; }

}
