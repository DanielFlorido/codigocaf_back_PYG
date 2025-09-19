
namespace CodigoCAFBack.Dominio.Contratos;

public class LineaBalanceGeneral
{

    public string Etiqueta { get; set; }

    public string Texto { get; set; }

    public int Mes { get; set; }

    public int Year { get; set; }

    public decimal Total { get; set; }

    public List<LineaBalanceGeneral> Data { get; set; }

}
