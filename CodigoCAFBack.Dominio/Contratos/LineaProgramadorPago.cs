
namespace CodigoCAFBack.Dominio.Contratos;

public class LineaProgramadorPago
{

    public Int32 Id { get; set; }

    public string Identificacion { get; set; }

    public string NombreProveedor { get; set; }

    public string Suc { get; set; }

    public string CentroCostos { get; set; }

    public string Ciudad { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }

    public string DetalleVencimiento {get; set; }
    
    public string NroCuota { get; set; }
    public DateOnly FechaVencimiento { get; set; }
    
    public Int32 DeudaPorPagar { get; set; }

    public decimal ValorAnticipo { get; set; }

    public decimal ValorAnticipoUSD { get; set; }

    public decimal SaldoProveedor { get; set; }

    public int TipoDocumento { get; set; }

    public string CodigoBanco { get; set; }

    public string CuentaBancaria { get; set; }

    public string CorreoElectronico { get; set; }

    public string TipoProveedor { get; set; }
}
