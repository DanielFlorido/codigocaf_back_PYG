
namespace CodigoCAFBack.Dominio.Contratos;

public class NotaBalanceGeneral
{

    public Int32 Id { get; set; }

    public string Account { get; set; }
    
    public string NoteHead { get; set; }
    
    public Double NoteValue { get; set; }
    
    public string NoteContent { get; set; } 
    
    public Int32 Year { get; set; }

    public Int32 IdCompany { get; set; }

    public Int32 Month { get; set; }

}
