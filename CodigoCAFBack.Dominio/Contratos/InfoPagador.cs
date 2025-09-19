
namespace CodigoCAFBack.Dominio.Contratos;

public class InfoPagador
{
    public string NumeroDocumento { get; set; }

    public string RazonSocial { get; set; }

    public int TipoDocumento { get; set; }

    public string NombreBanco { get; set; }

    public string CodigoBanco { get; set; }

    public string CuentaBancaria { get; set; }

    public string CorreoElectronico { get; set; }

    public string validarCuentaBancaria()
    {
        return CuentaBancaria == null ? "" : CuentaBancaria;
    }

    public string validarNombreBanco()
    {
        return NombreBanco == null || !NombreBanco.ToLower().Equals("bancolombia") ? "N" : "S";
    }

}
