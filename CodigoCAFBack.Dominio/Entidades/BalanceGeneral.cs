
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodigoCAFBack.Dominio.Entidades;

[Table("BalanceGeneral")]
public class BalanceGeneral
{

    [Key]
    public Int32 Id { get; set; }

    public string Nivel { get; set; }

    public bool Transaccional { get; set; }

    public string CodigoCuentaConble { get; set; }

	public string NombreCuentaConble { get; set; }
	
    public string Identificacion { get; set; }

    public string Sucursal { get; set; }

    public string NombreTercero { get; set; }

    public Double SaldoInicial { get; set; }

    public Double MovimientoDebito { get; set; }

    public Double MovimientoCredito { get; set; }

    public Double SaldoFinal { get; set; }

    public DateOnly Fecha { get; set; }

	public Int16 Mes { get; set; }
	
    public Int16 Year { get; set; }
	public string CentroCostos { get; set; }

    public string PAIS { get; set; }

    public string CuentaContableEs { get; set; }

    public string CodigoEbitda { get; set; }

    public string Large { get; set; }

    public string Tipo { get; set; }


    public string Grupo { get; set; }

    public Double MovimientoMes { get; set; }

    public string CodigoPosPre { get; set; }

    public string PosPreEn { get; set; }

    public string PosPreEs { get; set; }

    public string EBITDA { get; set; }

    public string TipoTerceroEs { get; set; }

    public string TipoTerceroEn { get; set; }

    public string Nit { get; set; }

    public string NombreCliente { get; set; }

    public Int32 IdCliente { get; set; }

}
