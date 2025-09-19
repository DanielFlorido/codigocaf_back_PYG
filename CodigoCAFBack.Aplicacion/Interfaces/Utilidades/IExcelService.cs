using CodigoCAFBack.Dominio.Comun;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Aplicacion.Interfaces.Utils;

public interface IExcelService
{
    public byte[] GenerarExcel(int idCliente, BalanceGeneralRequest balanceGeneralRequest, LogoCliente logoCliente, PYGResultado
        resultado, List<ResultadoUsuarioMulticliente> clientes, string meses, 
        PYGResultado balanceGeneralResultado);
    public int VisualizarLineaBalanceGeneral(int IdCliente, BalanceGeneralRequest balanceGeneralRequest,
        LineaBalanceGeneral lineaBalanceGeneral, IWorkbook workbook, ISheet sheet, int row, 
        ICellStyle labelStyle, ICellStyle dataStyle, ICellStyle mainLabelStyle, ICellStyle mainDataStyle,
        PYGResultado pygResultado);
    public int ProcesarRespuetas(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest, string newFilter,
        IWorkbook workbook, ISheet sheet, int row, ICellStyle labelStyle, ICellStyle dataStyle, 
        PYGResultado pygResultado);
}
