namespace CodigoCAFBack.Dominio.Contratos;

public class PYGResultado
{
    public List<string> Etiquetas { get; set; }

    public List<LineaBalanceGeneral> Data { get; set; }

    public List<LineaBalanceGeneral> CurrencyData { get; set; }

    public List<LineaBalanceGeneral> NotCurrencyData { get; set; }

    public LineaBalanceGeneral CurrencyTotal { get; set; }

    public LineaBalanceGeneral NotCurrencyTotal { get; set; }

    public LineaBalanceGeneral UtilidadBruta { get; set; }

    public LineaBalanceGeneral UtilidadOperacional { get; set; }

    public LineaBalanceGeneral UtilidadAntesImpuestos { get; set; }

    public LineaBalanceGeneral UtilidadNeta { get; set; }

}
