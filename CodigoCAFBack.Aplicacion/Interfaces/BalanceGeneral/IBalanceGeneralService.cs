

using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;

public interface IBalanceGeneralService
{

    List<BalanceGeneralAnio> ListarAnioBalanceGeneral(Int32 idCliente);

    BalanceGeneralResultado ConsultarEncabezado(Int32 IdCliente, BalanceGeneralRequest balanceGeneralRequest);

    BalanceGeneralResultado ConsultarSubEncabezado(Int32 IdCliente, BalanceGeneralRequest balanceGeneralRequest);

    BalanceGeneralResultado ConsultarDetalle(Int32 IdCliente, BalanceGeneralRequest balanceGeneralRequest);

    BalanceGeneralResultado ConsultarTercero(Int32 IdCliente, BalanceGeneralRequest balanceGeneralRequest);

    BalanceGeneralResultadoGrupo ConsultarEncabezadoAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    BalanceGeneralResultadoGrupo ConsultarSubEncabezadoAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    BalanceGeneralResultadoGrupo ConsultarDetalleAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    BalanceGeneralResultadoGrupo ConsultarTerceroAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    NotaBalanceGeneral GuardarNota(Int32 clienteId, NotaBalanceGeneralRequest notaBalanceGeneralRequest);

    byte[] GenerarExcel(Int32 IdCliente, BalanceGeneralRequest balanceGeneralRequest);

    byte[] GenerarExcelAgrupado(Int32 IdCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    void EnviarExcel(Int32 idCliente, BalanceGeneralRequest request);

    void EnviarExcelAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

}
