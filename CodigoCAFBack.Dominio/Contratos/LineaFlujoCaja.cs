
namespace CodigoCAFBack.Dominio.Contratos;

public class LineaFlujoCaja
{
    public string Tipo { get; set; }

    public string ShowText { get; set; }

    public Int32 Mes { get; set; }

    public decimal Value { get; set; }

    public List<LineaFlujoCaja> Data { get; set; }

}
