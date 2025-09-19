using CodigoCAFBack.Dominio.Entidades;

namespace CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;

public interface IParametrosBalanceGeneralService
{

    List<ParametroBalanceGeneral> ListarParametros(Int32 clienteId);

    List<ParametroBalanceGeneral> ListarParametrosPorIdPadre(Int32 clienteId, Nullable<Int32> idPadre);

    ParametroBalanceGeneral CrearParametro(ParametroBalanceGeneral parametroBalanceGeneral);

    ParametroBalanceGeneral EditarParametro(ParametroBalanceGeneral parametroBalanceGeneral);

    void EliminarParametro(Int32 clienteId, Int32 id);

}
