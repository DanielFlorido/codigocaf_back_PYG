using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Aplicacion.Interfaces.PYG;
public interface IPYGService
{
        PYGResultado ConsultarPYGGrupos(int idCliente, BalanceGeneralRequest request);

        PYGResultado ConsultarPYGCuentas(int idCliente, BalanceGeneralRequest request);

        PYGResultado ConsultarPYGSubCuentas(int idCliente, BalanceGeneralRequest request);

        PYGResultado ConsultarPYGAuxiliares(int idCliente, BalanceGeneralRequest request);

        byte[] GenerarExcel(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest);

        void EnviarExcel(Int32 idCliente, BalanceGeneralRequest request);

        string TituloCorreo(string Lenguaje);

        string CuerpoCorreo(string Lenguaje);
}

