

using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IProgramadorPagoService
{

    List<LineaProgramadorPago> ListarProgramador(Int32 idCliente, ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest);

    string GenerarCSV(Int32 idCliente, ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest);

}
