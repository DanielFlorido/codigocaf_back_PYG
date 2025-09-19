
namespace CodigoCAFBack.Dominio.Request;

public class NotaBalanceGeneralRequest
{

    public string Account { get; set; }
    
    public string NoteHead { get; set; }
    
    public Double NoteValue { get; set; }
    
    public string NoteContent { get; set; } 
    
    public Int32 Year { get; set; }
    
    public Int32 Month { get; set; }

}
