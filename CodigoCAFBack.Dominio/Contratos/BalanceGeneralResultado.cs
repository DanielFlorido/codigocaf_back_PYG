
namespace CodigoCAFBack.Dominio.Contratos;

public class BalanceGeneralResultado
{

    public List<string> Etiquetas { get; set; }

    public List<LineaBalanceGeneral> Data { get; set; }

    public List<LineaBalanceGeneral> CurrencyData  { get; set; }

    public List<LineaBalanceGeneral> NotCurrencyData { get; set; }

    public LineaBalanceGeneral CurrencyTotal { get; set; }

    public LineaBalanceGeneral NotCurrencyTotal { get; set; }

    public LineaBalanceGeneral UtilidadMes { get; set; }

    public LineaBalanceGeneral UtilidadAcumulada { get; set; }


}
