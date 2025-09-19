

using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;

public interface IProgramadorPagoRepository
{

    List<LineaProgramadorPago> ListarProgramador(Int32 idCliente, ConsultarBuscadorPagoRequest consultarBuscadorPagoRequest);

    InfoPagador ConsultarInfoPagador(Int32 idCliente);

}
