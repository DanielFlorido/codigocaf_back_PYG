
namespace CodigoCAFBack.Dominio.Contratos;

public class FlujoCajaResultado
{

    public List<string> Meses { get; set; }

    public List<LineaFlujoCaja> LineaFlujoCajas { get; set; }

    public List<LineaFlujoCajaTercero> LineaFlujoCajaTerceros { get; set; }

}
