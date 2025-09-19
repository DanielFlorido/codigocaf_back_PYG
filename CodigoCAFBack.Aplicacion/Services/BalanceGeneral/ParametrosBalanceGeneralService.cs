
using CodigoCAFBack.Aplicacion.Excepciones;
using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Dominio.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace CodigoCAFBack.Aplicacion.Services.BalanceGeneral;

public class ParametrosBalanceGeneralService : IParametrosBalanceGeneralService
{

    private readonly IParametrosBalanceGeneralRepository _parametrosBalanceGeneralRepository;

    public ParametrosBalanceGeneralService(IParametrosBalanceGeneralRepository parametrosBalanceGeneralRepository)
    {
        _parametrosBalanceGeneralRepository = parametrosBalanceGeneralRepository;
    }

    public List<ParametroBalanceGeneral> ListarParametros(int clienteId)
    {
        return _parametrosBalanceGeneralRepository.ListarParametros(clienteId);
    }

    public List<ParametroBalanceGeneral> ListarParametrosPorIdPadre(int clienteId, Nullable<Int32> idPadre)
    {
        return _parametrosBalanceGeneralRepository.ListarParametrosPorIdPadre(clienteId, idPadre);
    }

    public ParametroBalanceGeneral CrearParametro(ParametroBalanceGeneral parametroBalanceGeneral)
    {
        parametroBalanceGeneral.Id = _parametrosBalanceGeneralRepository.CrearParametro(parametroBalanceGeneral);
        return parametroBalanceGeneral;
    }

    public ParametroBalanceGeneral EditarParametro(ParametroBalanceGeneral parametroBalanceGeneral)
    {
        _parametrosBalanceGeneralRepository.EditarParametro(parametroBalanceGeneral);
        return parametroBalanceGeneral;
    }

    public void EliminarParametro(int clienteId, int id)
    {

        if(this.ListarParametrosPorIdPadre(clienteId, id).Count < 1) {
            _parametrosBalanceGeneralRepository.EliminarParametro(clienteId, id);
        } else
        {
            throw new CodigoCAFExcepcion("No se puede eliminar, tiene parámetro asociado");
        }
    }
}
