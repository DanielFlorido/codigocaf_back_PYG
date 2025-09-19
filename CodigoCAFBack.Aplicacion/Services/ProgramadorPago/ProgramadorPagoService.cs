

using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using System.Text;

namespace CodigoCAFBack.Aplicacion.Services.ProgramadorPago;

public class ProgramadorPagoService : IProgramadorPagoService
{
    private readonly IProgramadorPagoRepository _programadorPagoRepository;
    public ProgramadorPagoService(IProgramadorPagoRepository programadorPagoRepository)
    {
        _programadorPagoRepository = programadorPagoRepository;
    }

    public List<LineaProgramadorPago> ListarProgramador(Int32 idCliente, ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest)
    {
        return _programadorPagoRepository.ListarProgramador(idCliente, consultarBuscadorPagoRequest);
    }

    public string GenerarCSV(Int32 idCliente, ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest)
    {
        DateTime today = DateTime.Today;
        StringBuilder csv = new StringBuilder();
        InfoPagador infoPagador = _programadorPagoRepository.ConsultarInfoPagador(idCliente);
        csv.AppendLine("NIT PAGADOR;TIPO DE PAGO;APLICACIÓNS;SECUENCIA DE ENVÍO;NRO CUENTA A DEBITAR;TIPO DE CUENTA A DEBITAR;DESCRIPCIÓN DEL PAGO");
        csv.AppendLine(infoPagador.NumeroDocumento+";220;I;A1;"+infoPagador.validarCuentaBancaria()+";"+infoPagador.validarNombreBanco()+";Pr" + today.Year + today.Month + today.Day);
        csv.AppendLine("Tipo DocumentoBeneficiario;Nit Beneficiario;Nombre del Beneficiario;Tipo transacción;Código Banco;No Cuenta Beneficiario;Email;Documento autorizado;Referencia;OficinaEntrega;Valor;Transaccion;Fecha de aplicacion");
        List<LineaProgramadorPago> lista = ListarProgramador(idCliente, consultarBuscadorPagoRequest);
        foreach (var item in lista)
        {
            csv.AppendLine(item.TipoDocumento + ";" + item.Identificacion + ";" + item.NombreProveedor + ";37;" + item.CodigoBanco + ";" + item.CuentaBancaria + ";" + item.CorreoElectronico + ";;"+item.DetalleVencimiento+";;" + item.ValorAnticipo + ";;" + today.Year + today.Month + today.Day);
        }
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(csv.ToString()));
    }
}   
