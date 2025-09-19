
namespace CodigoCAFBack.Dominio.Contratos;

public class BalanceGeneralResultadoGrupo
{

    public List<string> Etiquetas { get; set; }

    public List<LineaBalanceGeneralAgrupado> Data { get; set; }

    public List<LineaBalanceGeneralAgrupado> CurrencyData { get; set; }

    public List<LineaBalanceGeneralAgrupado> NotCurrencyData { get; set; }

    public LineaBalanceGeneralAgrupado CurrencyTotal { get; set; }

    public LineaBalanceGeneralAgrupado NotCurrencyTotal { get; set; }

}
