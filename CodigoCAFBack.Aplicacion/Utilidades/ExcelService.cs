using CodigoCAFBack.Aplicacion.Interfaces.Utils;
using CodigoCAFBack.Dominio.Comun;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Aplicacion.Utilidades;

public class ExcelService : IExcelService
{
    public byte[] GenerarExcel(int idCliente, BalanceGeneralRequest balanceGeneralRequest, LogoCliente logoCliente, PYGResultado 
        resultado, List<ResultadoUsuarioMulticliente> clientes, string meses, PYGResultado pygResultado)
    {
        balanceGeneralRequest.Type = null;
        balanceGeneralRequest.Grupo = null;
        balanceGeneralRequest.CodigoPosPre = null;
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("PYG");

        sheet.IsPrintGridlines = false;
        sheet.DisplayGridlines = false;
        sheet.DefaultRowHeight = 400;

        

        //var logoCliente = _clienteLogoService.BuscarLogoCliente(logoClienteRequest);

        sheet.SetColumnWidth(0, 6000);
        sheet.SetColumnWidth(1, 4000);
        sheet.SetColumnWidth(2, 5000);
        sheet.SetColumnWidth(3, 5000);
        sheet.SetColumnWidth(4, 5000);
        sheet.SetColumnWidth(5, 4000);

        var rowTitle = sheet.CreateRow(0);
        rowTitle.Height = 1200;
        var titleCell = rowTitle.CreateCell(0);
        titleCell.SetCellValue("PYG SHEET");

        var titleFont = workbook.CreateFont();
        titleFont.FontHeightInPoints = 18;
        titleFont.IsBold = true;
        titleFont.Color = IndexedColors.White.Index;

        var titleStyle = workbook.CreateCellStyle();
        titleStyle.SetFont(titleFont);
        titleStyle.Alignment = HorizontalAlignment.Center;
        titleStyle.VerticalAlignment = VerticalAlignment.Center;
        titleStyle.FillForegroundColor = IndexedColors.RoyalBlue.Index;
        titleStyle.FillPattern = FillPattern.SolidForeground;

        titleCell.CellStyle = titleStyle;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 5));

        if (logoCliente != null)
        {
            if (logoCliente.LogoPrincipal != null && logoCliente.LogoPrincipal.Length > 0)
            {
                int logoIdx = workbook.AddPicture(logoCliente.LogoPrincipal, PictureType.PNG);
                var patriarch = sheet.CreateDrawingPatriarch();
                var anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, 0, 1, 1);
                anchor.AnchorType = AnchorType.MoveAndResize;
                var picture = patriarch.CreatePicture(anchor, logoIdx);
                picture.Resize(1.0);
            }

            if (logoCliente.LogoAuxiliar != null && logoCliente.LogoAuxiliar.Length > 0)
            {
                int logoAuxIdx = workbook.AddPicture(logoCliente.LogoAuxiliar, PictureType.PNG);
                var patriarch = sheet.CreateDrawingPatriarch();
                var auxAnchor = new XSSFClientAnchor(0, 0, 0, 0, 5, 0, 6, 1);
                auxAnchor.AnchorType = AnchorType.MoveAndResize;
                var auxPicture = patriarch.CreatePicture(auxAnchor, logoAuxIdx);
                auxPicture.Resize(1.0);
            }
        }

        var subtitleFont = workbook.CreateFont();
        subtitleFont.FontHeightInPoints = 10;
        subtitleFont.IsItalic = true;

        var rowHeader = sheet.CreateRow(4);
        rowHeader.Height = 500;

        var headerFont = workbook.CreateFont();
        headerFont.FontHeightInPoints = 11;
        headerFont.IsBold = true;
        headerFont.Color = IndexedColors.White.Index;

        var headerStyle = workbook.CreateCellStyle();
        headerStyle.SetFont(headerFont);
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.VerticalAlignment = VerticalAlignment.Center;
        headerStyle.FillForegroundColor = IndexedColors.RoyalBlue.Index;
        headerStyle.FillPattern = FillPattern.SolidForeground;
        headerStyle.BorderTop = BorderStyle.Medium;
        headerStyle.BorderBottom = BorderStyle.Medium;
        headerStyle.BorderLeft = BorderStyle.Medium;
        headerStyle.BorderRight = BorderStyle.Medium;
        headerStyle.TopBorderColor = IndexedColors.White.Index;
        headerStyle.BottomBorderColor = IndexedColors.White.Index;
        headerStyle.LeftBorderColor = IndexedColors.White.Index;
        headerStyle.RightBorderColor = IndexedColors.White.Index;

        var dataCellStyle = workbook.CreateCellStyle();
        dataCellStyle.Alignment = HorizontalAlignment.Right;
        dataCellStyle.BorderTop = BorderStyle.Thin;
        dataCellStyle.BorderBottom = BorderStyle.Thin;
        dataCellStyle.BorderLeft = BorderStyle.Thin;
        dataCellStyle.BorderRight = BorderStyle.Thin;
        dataCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

        var labelCellStyle = workbook.CreateCellStyle();
        labelCellStyle.Alignment = HorizontalAlignment.Left;
        labelCellStyle.BorderTop = BorderStyle.Thin;
        labelCellStyle.BorderBottom = BorderStyle.Thin;
        labelCellStyle.BorderLeft = BorderStyle.Thin;
        labelCellStyle.BorderRight = BorderStyle.Thin;

        var altRowStyle = workbook.CreateCellStyle();
        altRowStyle.CloneStyleFrom(dataCellStyle);
        altRowStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
        altRowStyle.FillPattern = FillPattern.SolidForeground;

        var altLabelStyle = workbook.CreateCellStyle();
        altLabelStyle.CloneStyleFrom(labelCellStyle);
        altLabelStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
        altLabelStyle.FillPattern = FillPattern.SolidForeground;

        var totalStyle = workbook.CreateCellStyle();
        totalStyle.CloneStyleFrom(dataCellStyle);
        totalStyle.FillForegroundColor = IndexedColors.RoyalBlue.Index;
        totalStyle.FillPattern = FillPattern.SolidForeground;
        totalStyle.TopBorderColor = IndexedColors.White.Index;
        totalStyle.BottomBorderColor = IndexedColors.White.Index;
        totalStyle.LeftBorderColor = IndexedColors.White.Index;
        totalStyle.RightBorderColor = IndexedColors.White.Index;

        var totalFont = workbook.CreateFont();
        totalFont.IsBold = true;
        totalStyle.SetFont(totalFont);
        totalFont.Color = IndexedColors.White.Index;

        var totalLabelStyle = workbook.CreateCellStyle();
        totalLabelStyle.CloneStyleFrom(labelCellStyle);
        totalLabelStyle.FillForegroundColor = IndexedColors.RoyalBlue.Index;
        totalLabelStyle.FillPattern = FillPattern.SolidForeground;
        totalLabelStyle.SetFont(totalFont);
        totalLabelStyle.TopBorderColor = IndexedColors.White.Index;
        totalLabelStyle.BottomBorderColor = IndexedColors.White.Index;
        totalLabelStyle.LeftBorderColor = IndexedColors.White.Index;
        totalLabelStyle.RightBorderColor = IndexedColors.White.Index;

        //BalanceGeneralResultado resultado = ConsultarEncabezado(idCliente, balanceGeneralRequest);
        //var cliente = _usuarioMulticlienteService.ConsultarClientesPorUsuario(balanceGeneralRequest.UserId).Find(client => client.IDCLIENTE == idCliente);
        var cliente = clientes.Find(client => client.IDCLIENTE == idCliente);
        var rowClient = sheet.CreateRow(3);
        rowClient.Height = 500;
        var cellClient = rowClient.CreateCell(0);
        sheet.SetColumnWidth(0, 12000);
        cellClient.SetCellValue(cliente.RazonSocial);
        cellClient.CellStyle = headerStyle;
        cellClient = rowClient.CreateCell(1);
        sheet.SetColumnWidth(0, 4000);
        cellClient.SetCellValue(cliente.NumeroDocumento);
        cellClient.CellStyle = headerStyle;
        cellClient = rowClient.CreateCell(2);
        sheet.SetColumnWidth(0, 4000);
        cellClient.SetCellValue(balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value.ToString() : (balanceGeneralRequest.DateFrom != null ? balanceGeneralRequest.DateFrom : ""));
        cellClient.CellStyle = headerStyle;
        cellClient = rowClient.CreateCell(3);
        sheet.SetColumnWidth(0, 4000);
        var month = balanceGeneralRequest.Year.HasValue ? 
            balanceGeneralRequest.Month.HasValue ? meses : "es".Equals(balanceGeneralRequest.Language) ? "Todos Los Meses" : "Every Month" : "";
        cellClient.SetCellValue(month.Length == 0 ? (balanceGeneralRequest.DateTo != null ? balanceGeneralRequest.DateTo : "") : month);
        cellClient.CellStyle = headerStyle;
        var colIndex = 0;
        foreach (var property in resultado.Etiquetas)
        {
            var cell = rowHeader.CreateCell(colIndex);
            if (colIndex == 0)
                sheet.SetColumnWidth(colIndex, 12000);
            else if (resultado.Etiquetas.Count - 1 == colIndex)
                sheet.SetColumnWidth(colIndex, 6000);
            else
                sheet.SetColumnWidth(colIndex, 4000);
            cell.SetCellValue(property);
            cell.CellStyle = headerStyle;
            colIndex++;
        }

        balanceGeneralRequest.etiquetas = resultado.Etiquetas;
        int row = 5;
        bool isAlternateRow = false;

        foreach (var item in resultado.Data)
        {
            row = VisualizarLineaBalanceGeneral(idCliente, new BalanceGeneralRequest()
            {
                etiquetas = balanceGeneralRequest.etiquetas,
                Language = balanceGeneralRequest.Language,
                Month = balanceGeneralRequest.Month,
                Year = balanceGeneralRequest.Year,
                DateFrom = balanceGeneralRequest.DateFrom,
                DateTo = balanceGeneralRequest.DateTo,
                Type = item.Etiqueta,
                Grupo = null,
                CodigoPosPre = null,
                UserId = balanceGeneralRequest.UserId,
                IncludeSignature = balanceGeneralRequest.IncludeSignature,
            }, item, workbook, sheet, row,
                isAlternateRow ? altLabelStyle : labelCellStyle,
                isAlternateRow ? altRowStyle : dataCellStyle,
                item.Texto.Contains("Total") ? totalLabelStyle : (isAlternateRow ? altLabelStyle : labelCellStyle),
                item.Texto.Contains("Total") ? totalStyle : (isAlternateRow ? altRowStyle : dataCellStyle), pygResultado);

            isAlternateRow = !isAlternateRow;
            row++;
        }
        row--;
        var rowData = sheet.CreateRow(row);
        var labelCell = rowData.CreateCell(0);
        labelCell.SetCellValue(resultado.UtilidadBruta.Texto);
        labelCell.CellStyle = totalLabelStyle;

        colIndex = 1;
        foreach (var data in resultado.UtilidadBruta.Data)
        {
            var cell = rowData.CreateCell(colIndex);
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(double.Parse(data.Total.ToString()));
            cell.CellStyle = totalStyle;
            colIndex++;
        }

        row++;

        rowData = sheet.CreateRow(row);
        labelCell = rowData.CreateCell(0);
        labelCell.SetCellValue(resultado.UtilidadOperacional.Texto);
        labelCell.CellStyle = totalLabelStyle;

        colIndex = 1;
        foreach (var data in resultado.UtilidadOperacional.Data)
        {
            var cell = rowData.CreateCell(colIndex);
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(double.Parse(data.Total.ToString()));
            cell.CellStyle = totalStyle;
            colIndex++;
        }

        row++;

        rowData = sheet.CreateRow(row);
        labelCell = rowData.CreateCell(0);
        labelCell.SetCellValue(resultado.UtilidadAntesImpuestos.Texto);
        labelCell.CellStyle = totalLabelStyle;

        colIndex = 1;
        foreach (var data in resultado.UtilidadAntesImpuestos.Data)
        {
            var cell = rowData.CreateCell(colIndex);
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(double.Parse(data.Total.ToString()));
            cell.CellStyle = totalStyle;
            colIndex++;
        }

        row++;

        rowData = sheet.CreateRow(row);
        labelCell = rowData.CreateCell(0);
        labelCell.SetCellValue(resultado.UtilidadNeta.Texto);
        labelCell.CellStyle = totalLabelStyle;

        colIndex = 1;
        foreach (var data in resultado.UtilidadNeta.Data)
        {
            var cell = rowData.CreateCell(colIndex);
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(double.Parse(data.Total.ToString()));
            cell.CellStyle = totalStyle;
            colIndex++;
        }

        var footerRow = sheet.CreateRow(row + 2);
        footerRow.Height = 400;
        var footerCell = footerRow.CreateCell(0);
        footerCell.SetCellValue("Generado el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

        var footerStyle = workbook.CreateCellStyle();
        footerStyle.Alignment = HorizontalAlignment.Left;
        footerStyle.VerticalAlignment = VerticalAlignment.Center;
        var footerFont = workbook.CreateFont();
        footerFont.IsItalic = true;
        footerFont.Color = IndexedColors.Grey50Percent.Index;
        footerStyle.SetFont(footerFont);
        footerCell.CellStyle = footerStyle;

        if (balanceGeneralRequest.IncludeSignature && logoCliente != null)
        {
            int signatureRow = row + 5;
            int signatureHeight = 2;

            if (logoCliente.FirmaContador != null && logoCliente.FirmaContador.Length > 0)
            {
                int contador = workbook.AddPicture(logoCliente.FirmaContador, PictureType.PNG);
                var patriarch = sheet.CreateDrawingPatriarch();
                var contadorAnchor = new XSSFClientAnchor(0, 0, 0, 0, 0, signatureRow, 2, signatureRow + signatureHeight);
                var contadorPic = patriarch.CreatePicture(contadorAnchor, contador);
                contadorPic.Resize(0.8);

                var contadorLabelRow = sheet.CreateRow(signatureRow + signatureHeight);
                var contadorLabel = contadorLabelRow.CreateCell(0);
                contadorLabel.SetCellValue(balanceGeneralRequest.Language.Equals("es") ?
                    "Contador" : "Accountant");
                contadorLabel.CellStyle = footerStyle;
            }

            if (logoCliente.FirmaRevisor != null && logoCliente.FirmaRevisor.Length > 0)
            {
                int revisor = workbook.AddPicture(logoCliente.FirmaRevisor, PictureType.PNG);
                var patriarch = sheet.CreateDrawingPatriarch();
                var revisorAnchor = new XSSFClientAnchor(0, 0, 0, 0, 2, signatureRow, 4, signatureRow + signatureHeight);
                var contadorPic = patriarch.CreatePicture(revisorAnchor, revisor);
                contadorPic.Resize(0.8);

                var revisorLabelRow = sheet.GetRow(signatureRow + signatureHeight);
                if (revisorLabelRow == null) revisorLabelRow = sheet.CreateRow(signatureRow + signatureHeight);
                var revisorLabel = revisorLabelRow.CreateCell(2);
                revisorLabel.SetCellValue(balanceGeneralRequest.Language.Equals("es") ?
                    "Revisor Fiscal" : "Fiscal Reviewer");
                revisorLabel.CellStyle = footerStyle;
            }

            if (logoCliente.FirmaGerente != null && logoCliente.FirmaGerente.Length > 0)
            {
                int gerente = workbook.AddPicture(logoCliente.FirmaGerente, PictureType.PNG);
                var patriarch = sheet.CreateDrawingPatriarch();
                var gerenteAnchor = new XSSFClientAnchor(0, 0, 0, 0, 4, signatureRow, 6, signatureRow + signatureHeight);
                var contadorPic = patriarch.CreatePicture(gerenteAnchor, gerente);
                contadorPic.Resize(0.8);

                var gerenteLabelRow = sheet.GetRow(signatureRow + signatureHeight);
                if (gerenteLabelRow == null) gerenteLabelRow = sheet.CreateRow(signatureRow + signatureHeight);
                var gerenteLabel = gerenteLabelRow.CreateCell(4);
                gerenteLabel.SetCellValue(balanceGeneralRequest.Language.Equals("es") ?
                    "Gerente" : "Manager");
                gerenteLabel.CellStyle = footerStyle;
            }

            row = signatureRow + 3;
        }

        var noticeRow = sheet.CreateRow(row + 3);
        var noticeCell = noticeRow.CreateCell(0);
        noticeCell.SetCellValue(balanceGeneralRequest.Language.Equals("es") ?
            "Documento confidencial. Prohibida su reproducción parcial o total sin autorización." :
            "Confidential document. Reproduction in part or in whole is prohibited without authorization.");
        noticeCell.CellStyle = footerStyle;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(row + 3, row + 3, 0, 5));

        var stream = new MemoryStream();
        workbook.Write(stream);
        var content = stream.ToArray();

        return content;
    }

    public int VisualizarLineaBalanceGeneral(int IdCliente, BalanceGeneralRequest balanceGeneralRequest, LineaBalanceGeneral lineaBalanceGeneral, IWorkbook workbook, ISheet sheet, int row, ICellStyle labelStyle, ICellStyle dataStyle, ICellStyle mainLabelStyle, ICellStyle mainDataStyle, PYGResultado pygResultado)
    {
        var rowData = sheet.CreateRow(row);
        var labelCell = rowData.CreateCell(0);
        labelCell.SetCellValue(lineaBalanceGeneral.Texto);
        labelCell.CellStyle = mainLabelStyle;

        int colIndex = 1;
        foreach (var data in lineaBalanceGeneral.Data)
        {
            var cell = rowData.CreateCell(colIndex);
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(double.Parse(data.Total.ToString()));
            cell.CellStyle = mainDataStyle;
            colIndex++;
        }

        row++;

        if (!balanceGeneralRequest.Type.ToLower().Contains("total"))
        {
            row = ProcesarRespuetas(IdCliente, balanceGeneralRequest, lineaBalanceGeneral.Etiqueta, workbook, sheet, row, labelStyle, dataStyle, pygResultado);
        }
        return row; ;
    }
    public int ProcesarRespuetas(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest, string newFilter,
        IWorkbook workbook, ISheet sheet, int row, ICellStyle labelStyle, ICellStyle dataStyle, PYGResultado pygResultado)
    {
        ICellStyle indentLabelStyle = workbook.CreateCellStyle();
        indentLabelStyle.CloneStyleFrom(labelStyle);
        indentLabelStyle.Indention = 2;

        ICellStyle positiveValueStyle = workbook.CreateCellStyle();
        positiveValueStyle.CloneStyleFrom(dataStyle);
        var positiveFont = workbook.CreateFont();
        positiveFont.Color = IndexedColors.DarkGreen.Index;
        positiveValueStyle.SetFont(positiveFont);

        ICellStyle negativeValueStyle = workbook.CreateCellStyle();
        negativeValueStyle.CloneStyleFrom(dataStyle);
        var negativeFont = workbook.CreateFont();
        negativeFont.Color = IndexedColors.DarkRed.Index;
        negativeValueStyle.SetFont(negativeFont);

        ICellStyle personCellStyle = null;
        if (balanceGeneralRequest.Grupo != null)
        {
            personCellStyle = workbook.CreateCellStyle();
            personCellStyle.CloneStyleFrom(indentLabelStyle);
            personCellStyle.Indention = 4;
            var personFont = workbook.CreateFont();
            personFont.Underline = FontUnderlineType.Single;
            personCellStyle.SetFont(personFont);
        }
        else if (balanceGeneralRequest.CodigoPosPre != null)
        {
            personCellStyle = workbook.CreateCellStyle();
            personCellStyle.CloneStyleFrom(indentLabelStyle);
            personCellStyle.Indention = 5;
            var personFont = workbook.CreateFont();
            personFont.IsItalic = true;
            personCellStyle.SetFont(personFont);
        }

        bool isAlternateRow = false;
        foreach (var linea in pygResultado.Data)
        {
            var rowData = sheet.CreateRow(row);
            var labelCell = rowData.CreateCell(0);
            labelCell.SetCellValue(linea.Texto);

            if (personCellStyle != null && !string.IsNullOrEmpty(linea.Texto) &&
                (linea.Texto.Contains(" - ") || char.IsUpper(linea.Texto[0])))
            {
                labelCell.CellStyle = personCellStyle;
            }
            else
            {
                labelCell.CellStyle = indentLabelStyle;
            }

            int colIndex = 1;
            foreach (var data in linea.Data)
            {
                var cell = rowData.CreateCell(colIndex);
                cell.SetCellType(CellType.Numeric);
                double value = double.Parse(data.Total.ToString());
                cell.SetCellValue(value);

                if (value > 0)
                    cell.CellStyle = positiveValueStyle;
                else if (value < 0)
                    cell.CellStyle = negativeValueStyle;
                else
                    cell.CellStyle = dataStyle;

                colIndex++;
            }
            row++;
            BalanceGeneralRequest deeperRequest = new BalanceGeneralRequest()
            {
                etiquetas = balanceGeneralRequest.etiquetas,
                DateFrom = balanceGeneralRequest.DateFrom,
                DateTo = balanceGeneralRequest.DateTo,
                Language = balanceGeneralRequest.Language,
                Month = balanceGeneralRequest.Month,
                Year = balanceGeneralRequest.Year,
                Grupo = string.IsNullOrEmpty(balanceGeneralRequest.Type) ? null : linea.Etiqueta,
                CodigoPosPre = string.IsNullOrEmpty(balanceGeneralRequest.Grupo) ? null : linea.Etiqueta,
                IncludeSignature = balanceGeneralRequest.IncludeSignature,
                UserId = balanceGeneralRequest.UserId,
            };
            row = ProcesarRespuetas(idCliente, deeperRequest, linea.Etiqueta,
                                                    workbook, sheet, row, labelStyle, dataStyle, pygResultado);

            isAlternateRow = !isAlternateRow;
        }
        return row;
    }

}
