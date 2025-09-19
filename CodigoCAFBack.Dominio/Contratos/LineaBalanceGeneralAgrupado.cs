
namespace CodigoCAFBack.Dominio.Contratos;

public class LineaBalanceGeneralAgrupado
{
    public string Codigo { get; set; }

    public string Texto { get; set; }

    public Nullable<Int32> Notas { get; set; }

    public decimal ValorPrevio { get; set; }

    public decimal USDPrevio { get; set; }

    public decimal ValorActual { get; set; }

    public decimal USDActual { get; set; }

    public decimal ValorVariacion { get; set; }

    public decimal USDVariacion { get; set; }

    public decimal ValorPorcentaje { get; set; }

}
