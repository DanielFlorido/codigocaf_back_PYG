
using CodigoCAFBack.Dominio.Entidades;

namespace CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;

public interface IParametrosBalanceGeneralRepository
{

    List<ParametroBalanceGeneral> ListarParametros(Int32 clienteId);

    List<ParametroBalanceGeneral> ListarParametrosPorIdPadre(Int32 clienteId, Nullable<Int32> idPadre);

    Int32 CrearParametro(ParametroBalanceGeneral parametroBalanceGeneral);

    void EditarParametro(ParametroBalanceGeneral parametroBalanceGeneral);

    void EliminarParametro(Int32 clienteId, Int32 id);

}
