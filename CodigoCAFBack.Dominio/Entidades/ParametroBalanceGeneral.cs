using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodigoCAFBack.Dominio.Entidades;

[Table("ParametrosBalanceGeneral")]
public class ParametroBalanceGeneral
{

    [Key]
    public Int32 Id { get; set; }

    public string Codigo { get; set; }

    public string NombreEs { get; set; }

    public string NombreEn { get; set; }

    public string PosPreEs { get; set; }

    public string PosPreEn { get; set; }

    public bool AmortizacionIntereses { get; set; }

    public bool EsCorriente { get; set; }

    public bool FlujoCaja { get; set; }

    public Nullable<Int32> IdPadre { get; set; }

    public Int32 IdCliente { get; set; }

}
