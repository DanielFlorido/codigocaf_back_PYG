
namespace CodigoCAFBack.Dominio.Contratos;

public class LineaFlujoCajaTercero
{
    public string Identificacion { get; set; }

    public string NombreTercero { get; set; }

    public string POSPRE { get; set; }

    public Int32 Mes { get; set; }

    public decimal Value { get; set; }

    public List<LineaFlujoCajaTercero> Data { get; set; }

}
