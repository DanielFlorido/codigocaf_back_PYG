

using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;

namespace CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;

public interface IBalanceGeneralRepository
{

    List<BalanceGeneralAnio> ListarAnioBalanceGeneral(Int32 idCliente);

    List<LineaBalanceGeneral> BuscarEncabezado(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest);

    List<LineaBalanceGeneral> BuscarSubEncabezado(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest);

    List<GrupoClasificacionCorriente> BuscarClasificacionCorriente(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest, bool esCorriente);

    List<LineaBalanceGeneral> BuscarDetalle(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest);

    List<LineaBalanceGeneral> BuscarTerceros(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest);

    List<LineaBalanceGeneralAnio> BuscarEncabezadoAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    List<LineaBalanceGeneralAnio> BuscarSubEncabezadoAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    List<GrupoClasificacionCorriente> BuscarGrupoClasificacionCorriente(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest, bool esCorriente);

    List<LineaBalanceGeneralAnio> BuscarDetalleAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    List<LineaBalanceGeneralAnioDet> BuscarTerceroAnio(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest);

    Int32 GuardarNota(Int32 idCliente, NotaBalanceGeneralRequest notaBalanceGeneral);

}
