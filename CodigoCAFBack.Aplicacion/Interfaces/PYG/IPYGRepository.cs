using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Aplicacion.Interfaces.PYG
{
    public interface IPYGRepository
    {
        public List<LineaBalanceGeneral> ConsultarPYGGrupos(int idCliente, BalanceGeneralRequest balanceGeneralRequest);
        public List<LineaBalanceGeneral> ConsultarPYGCuentas(int idCliente, BalanceGeneralRequest balanceGeneralRequest);

        public List<LineaBalanceGeneral> ConsultarPYGSubcuentas(int idCliente, BalanceGeneralRequest balanceGeneralRequest);

        public List<LineaBalanceGeneral> ConsultarPYGAuxiliares(int idCliente, BalanceGeneralRequest balanceGeneralRequest);
        
    }
}
