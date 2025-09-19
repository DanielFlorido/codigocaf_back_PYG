using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;
using CodigoCAFBack.Aplicacion.Interfaces.Mail;
using CodigoCAFBack.Aplicacion.Services.Mail;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;

namespace CodigoCAFBack.Aplicacion.Services.FlujoCaja;

public class FlujoCajaService : IFlujoCajaService
{

    private readonly IFlujoCajaRepository _flujoCajaRepository;
    private readonly IClienteLogoService _clienteLogoService;
    private readonly IMesesService _mesesService;
    private readonly IMailService _mailService;
    private readonly IUsuarioMulticlienteService _usuarioMulticlienteService;
    public FlujoCajaService(IFlujoCajaRepository flujoCajaRepository, IClienteLogoService clienteLogoService, IMesesService mesesService, IMailService mailService, IUsuarioMulticlienteService usuarioMulticlienteService)
    {
        _flujoCajaRepository = flujoCajaRepository;
        _clienteLogoService = clienteLogoService;
        _mesesService = mesesService;
        _mailService = mailService;
        _usuarioMulticlienteService = usuarioMulticlienteService;
    }

    public List<FlujoCajaAnio> ListarAnio(Int32 idCliente)
    {
        return _flujoCajaRepository.ListarAnio(idCliente);
    }

    public FlujoCajaResultado ConsultarEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
    {
        FlujoCajaResultado flujoCajaResultado = new FlujoCajaResultado();
        flujoCajaResultado.Meses = new List<string>() { ObtenerEtiqueta(flujoCajaRequest.Language) };
        flujoCajaResultado.LineaFlujoCajas = new List<LineaFlujoCaja>();

        List<LineaFlujoCaja> lineaFlujoCajas = _flujoCajaRepository.BuscarEncabezado(idCliente, flujoCajaRequest);

        //Se obtienen los meses que se van a procesar en la base de datos
        foreach (var item in lineaFlujoCajas.GroupBy(x => x.Mes))
        {
            flujoCajaResultado.Meses.Add(_mesesService.ConsultarMes(flujoCajaRequest.Language, item.Key));
        }

        LineaFlujoCaja Total = new LineaFlujoCaja() { Tipo= NombreTotal(flujoCajaRequest.Language, flujoCajaRequest.IdentificacionTercero),
                                                        ShowText = NombreTotal(flujoCajaRequest.Language, flujoCajaRequest.IdentificacionTercero),
                                                        Data = new List<LineaFlujoCaja>()
        };
        if(!Total.Tipo.StartsWith("3"))
        {
            Total.Tipo = "none";
        }

        foreach (var item in lineaFlujoCajas.GroupBy(x => x.Tipo))
        {
            LineaFlujoCaja LineaFlujoCaja = new LineaFlujoCaja()
            {
                Tipo = item.ToList().First().Tipo,
                ShowText = item.ToList().First().ShowText,
                Mes = 0,
                Data = ProcesarMeses(flujoCajaResultado.Meses, item.Key, item.ToList(), flujoCajaRequest.Language)
            };
            Total.Data.Add(new LineaFlujoCaja()
            {
                Tipo = Total.Tipo,
                Value = LineaFlujoCaja.Data.Sum(x => x.Value)
            });
            flujoCajaResultado.LineaFlujoCajas.Add(LineaFlujoCaja);
        }
        if (lineaFlujoCajas.Count > 0) { 
            flujoCajaResultado.LineaFlujoCajas.Add(Total);
        } else
        {
            flujoCajaResultado.Meses = new List<string>();
        }
            return flujoCajaResultado;
    }

    public FlujoCajaResultado ConsultarSubEncabezado(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
    {
        FlujoCajaResultado flujoCajaResultado = new FlujoCajaResultado();

        List<LineaFlujoCaja> lineaFlujoCajas = _flujoCajaRepository.BuscarSubEncabezado(idCliente, flujoCajaRequest);

        flujoCajaResultado.LineaFlujoCajas = lineaFlujoCajas.GroupBy(x => x.Tipo).Select(item => {
            return new LineaFlujoCaja()
            {
                Tipo = item.ToList().First().Tipo,
                ShowText = item.ToList().First().ShowText,
                Mes = 0,
                Data = ProcesarMeses(flujoCajaRequest.Meses, item.Key, item.ToList(), flujoCajaRequest.Language)
            };
        }).ToList();
        return flujoCajaResultado;
    }

    public FlujoCajaResultado DetalleTercero(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
    {
        FlujoCajaResultado flujoCajaResultado = new FlujoCajaResultado();

        List<LineaFlujoCajaTercero> lineaFlujoCajaTerceros = _flujoCajaRepository.DetalleTercero(idCliente, flujoCajaRequest);

        flujoCajaRequest.Meses.Remove(ObtenerEtiqueta(flujoCajaRequest.Language));

        flujoCajaResultado.LineaFlujoCajaTerceros = lineaFlujoCajaTerceros.GroupBy(x => x.Identificacion).Select(item => {
            return new LineaFlujoCajaTercero()
            {
                Identificacion = item.ToList().First().Identificacion,
                NombreTercero = item.ToList().First().NombreTercero,
                POSPRE = item.ToList().First().POSPRE,
                Mes = 0,
                Data = flujoCajaRequest.Meses.Select(mes => {
                    var mesNum = _mesesService.ObtenerNumeroMes(mes, flujoCajaRequest.Language);
                    return new LineaFlujoCajaTercero()
                    {
                        Identificacion = item.ToList().First().Identificacion,
                        NombreTercero = item.ToList().First().NombreTercero,
                        POSPRE = item.ToList().First().POSPRE,
                        Mes = mesNum.HasValue ? 0 : mesNum.Value,
                        Value = item.ToList().FindAll(x => (x.Mes.ToString() == mes) || (_mesesService.ObtenerNombreMes(x.Mes, flujoCajaRequest.Language).Equals(mes))).Sum(x => x.Value)
                    };
                }).ToList()
            };
        }).ToList();
        return flujoCajaResultado;
    }

    public List<TerceroFlujoCaja> BuscarTercero(Int32 idCliente)
    {
        return _flujoCajaRepository.BuscarTercero(idCliente);
    }

    public void EnviarExcel(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
    {
        String title = "es".Equals(flujoCajaRequest.Language) ? "Reporte Flujo Caja" : "Cash Flow Report";
        String body = "es".Equals(flujoCajaRequest.Language) ? "Hola, adjunto encontraras el reporte flujo caja" : "Hello, please find attached the cash flow report.";
        _mailService.SendEmailAsync(_usuarioMulticlienteService.ObtenerCorreoParaCliente(idCliente), title, body, new List<byte[]>() { GenerarExcel(idCliente, flujoCajaRequest) }
            , new List<string>() { "FlujoCaja.xlsx" }, false);
    }

    public byte[] GenerarExcel(Int32 idCliente, FlujoCajaRequest flujoCajaRequest)
{
    var workbook = new XSSFWorkbook();
    var sheet = workbook.CreateSheet("Balance General");
    
    // Configuración básica de la hoja
    sheet.DisplayGridlines = false;
    sheet.IsPrintGridlines = false;
    sheet.DefaultRowHeight = 400;

    var logoClienteRequest = new LogoClienteRequest
    {
        IdCliente = idCliente,
        IdUsuario = flujoCajaRequest.UserId
    };

    var logoCliente = _clienteLogoService.BuscarLogoCliente(logoClienteRequest);

    sheet.SetColumnWidth(0, 6000);
    sheet.SetColumnWidth(1, 4000);
    sheet.SetColumnWidth(2, 4000);
    sheet.SetColumnWidth(3, 4000);
    sheet.SetColumnWidth(4, 4000);
    sheet.SetColumnWidth(5, 6000);

    var styles = CreateStyles(workbook);

    var headerRow = sheet.CreateRow(0);
    headerRow.Height = 1200;
    var titleCell = headerRow.CreateCell(0);
    titleCell.SetCellValue("INFORME DE FLUJO DE CAJA");
    titleCell.CellStyle = styles["Title"];
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

    var cliente = _usuarioMulticlienteService.ConsultarClientesPorUsuario(flujoCajaRequest.UserId).Find(client => client.IDCLIENTE == idCliente);
    var clientData = new LinkedList<string>() { };
    clientData.AddLast(cliente.RazonSocial);
    clientData.AddLast(cliente.NumeroDocumento);
    clientData.AddLast(flujoCajaRequest.Year.ToString());
    if (flujoCajaRequest.Month.HasValue)
        {
            clientData.AddLast(_mesesService.ObtenerNombreMes(flujoCajaRequest.Month.Value, flujoCajaRequest.Language));
        }
        else
        {
            clientData.AddLast("es".Equals(flujoCajaRequest.Language) ? "Todos los meses" : "Every Month");
        }
    int initialRow = 4;

        var rowHeaderClient = sheet.CreateRow(initialRow);
        rowHeaderClient.Height = 600;
        int col = 0;

        sheet.SetColumnWidth(0, 12000);
        foreach (var item in clientData)
        {
            var cell = rowHeaderClient.CreateCell(col);
            cell.SetCellValue(item);
            cell.CellStyle = styles["ColumnHeader"];
            if (col > 0)
            {
                sheet.SetColumnWidth(col, 4000);
            }
            col++;
        }
        initialRow++;

        FlujoCajaResultado flujoCajaResultado = ConsultarEncabezado(idCliente, flujoCajaRequest);

    var rowHeader = sheet.CreateRow(initialRow);
    rowHeader.Height = 600;
    col = 0;

    sheet.SetColumnWidth(0, 12000);

    foreach (var item in flujoCajaResultado.Meses)
    {
        var cell = rowHeader.CreateCell(col);
        cell.SetCellValue(item);
        cell.CellStyle = styles["ColumnHeader"];
        if (col > 0) {
            sheet.SetColumnWidth(col, 4000);
        }
        col++;
    }

    flujoCajaRequest.Meses = flujoCajaResultado.Meses;
    flujoCajaRequest.Tipo = null;
    flujoCajaRequest.CodigoPosPre = null;
    Int32 row = initialRow + 1;

    bool isAlternateRow = false;
    foreach (var item in flujoCajaResultado.LineaFlujoCajas)
    {
        row = PintarLinea(idCliente, flujoCajaRequest, sheet, item, row, styles, isAlternateRow);
        isAlternateRow = !isAlternateRow;
    }


    int lastRow = row + 2;
    var footerRow = sheet.CreateRow(lastRow);
    footerRow.Height = 400;
    var footerCell = footerRow.CreateCell(0);
    footerCell.SetCellValue("Generado el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
    footerCell.CellStyle = styles["Footer"];

    if (logoCliente != null)
    {
        int signatureRow = lastRow + 2;
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
            contadorLabel.SetCellValue(flujoCajaRequest.Language?.Equals("es") ?? true ? 
                "Contador" : "Accountant");
            contadorLabel.CellStyle = styles["SignatureLabel"];
        }

        if (logoCliente.FirmaRevisor != null && logoCliente.FirmaRevisor.Length > 0)
        {
            int revisor = workbook.AddPicture(logoCliente.FirmaRevisor, PictureType.PNG);
            var patriarch = sheet.CreateDrawingPatriarch();
            var revisorAnchor = new XSSFClientAnchor(0, 0, 0, 0, 2, signatureRow, 4, signatureRow + signatureHeight);
            var revisorPic = patriarch.CreatePicture(revisorAnchor, revisor);
            revisorPic.Resize(0.8);

            var revisorLabelRow = sheet.GetRow(signatureRow + signatureHeight);
            if (revisorLabelRow == null) revisorLabelRow = sheet.CreateRow(signatureRow + signatureHeight);
            var revisorLabel = revisorLabelRow.CreateCell(2);
            revisorLabel.SetCellValue(flujoCajaRequest.Language?.Equals("es") ?? true ? 
                "Revisor Fiscal" : "Fiscal Reviewer");
            revisorLabel.CellStyle = styles["SignatureLabel"];
        }

        if (logoCliente.FirmaGerente != null && logoCliente.FirmaGerente.Length > 0)
        {
            int gerente = workbook.AddPicture(logoCliente.FirmaGerente, PictureType.PNG);
            var patriarch = sheet.CreateDrawingPatriarch();
            var gerenteAnchor = new XSSFClientAnchor(0, 0, 0, 0, 4, signatureRow, 6, signatureRow + signatureHeight);
            var gerentePic = patriarch.CreatePicture(gerenteAnchor, gerente);
            gerentePic.Resize(0.8);

            var gerenteLabelRow = sheet.GetRow(signatureRow + signatureHeight);
            if (gerenteLabelRow == null) gerenteLabelRow = sheet.CreateRow(signatureRow + signatureHeight);
            var gerenteLabel = gerenteLabelRow.CreateCell(4);
            gerenteLabel.SetCellValue(flujoCajaRequest.Language?.Equals("es") ?? true ? 
                "Gerente" : "Manager");
            gerenteLabel.CellStyle = styles["SignatureLabel"];
        }

        lastRow = signatureRow + signatureHeight + 2;
    }

    var noticeRow = sheet.CreateRow(lastRow + 1);
    var noticeCell = noticeRow.CreateCell(0);
    noticeCell.SetCellValue("Documento confidencial. Prohibida su reproducción parcial o total sin autorización.");
    noticeCell.CellStyle = styles["Footer"];
    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(lastRow + 1, lastRow + 1, 0, 5));

    var stream = new MemoryStream();
    workbook.Write(stream);
    var content = stream.ToArray();

    return content;
}

public Int32 PintarLinea(Int32 idCliente, FlujoCajaRequest flujoCajaRequest, ISheet sheet, LineaFlujoCaja data, Int32 rowNumber, Dictionary<string, ICellStyle> styles, bool isAlternateRow)
{
    var rowHeader = sheet.CreateRow(rowNumber);
    rowHeader.Height = 450;
    int col = 0;

    var cell = rowHeader.CreateCell(col);
    cell.SetCellValue(data.ShowText);

    if (data.ShowText.StartsWith("3"))
    {
        cell.CellStyle = styles["TotalRow"];
    }
    else if (data.ShowText.StartsWith("1") || data.ShowText.StartsWith("2"))
    {
        cell.CellStyle = styles["CategoryRow"];
    }
    else
    {
        cell.CellStyle = isAlternateRow ? styles["AlternateRow"] : styles["NormalRow"];
    }

    foreach (var item in data.Data)
    {
        col++;
        var valueCell = rowHeader.CreateCell(col);
        try
        {
            valueCell.SetCellValue(Convert.ToDouble(item.Value));
        }
        catch
        {
            valueCell.SetCellValue(item.Value.ToString());
        }

        if (data.ShowText.StartsWith("3"))
        {
            valueCell.CellStyle = styles["TotalValue"];
        }
        else if (data.ShowText.StartsWith("1") || data.ShowText.StartsWith("2"))
        {
            valueCell.CellStyle = styles["CategoryValue"];
        }
        else
        {
            valueCell.CellStyle = isAlternateRow ? styles["AlternateValue"] : styles["NormalValue"];
        }
    }

    rowNumber++;

    if (data.ShowText.StartsWith("1") || data.ShowText.StartsWith("2") || flujoCajaRequest.Tipo != null)
    {
        rowNumber = ProcesarLinea(idCliente, flujoCajaRequest, sheet, data.Tipo, rowNumber, styles);
    }

    return rowNumber;
}

public Int32 PintarLineaTercero(Int32 idCliente, ISheet sheet, LineaFlujoCajaTercero data, Int32 rowNumber, Dictionary<string, ICellStyle> styles)
{
    var rowHeader = sheet.CreateRow(rowNumber);
    rowHeader.Height = 400;
    int col = 0;

    var cell = rowHeader.CreateCell(col);
    cell.SetCellValue(data.Identificacion + " - " + data.NombreTercero);
    cell.CellStyle = styles["DetailRow"];

    foreach (var item in data.Data)
    {
        col++;
        var valueCell = rowHeader.CreateCell(col);
        try
        {
            valueCell.SetCellValue(Convert.ToDouble(item.Value));
        }
        catch
        {
            valueCell.SetCellValue(item.Value.ToString());
        }
        valueCell.CellStyle = styles["DetailValue"];
    }
    
    rowNumber++;
    return rowNumber;
}

private Int32 ProcesarLinea(Int32 idCliente, FlujoCajaRequest flujoCajaRequest, ISheet sheet, String nuevoFiltro, Int32 row, Dictionary<string, ICellStyle> styles)
{
    if(flujoCajaRequest.Tipo == null)
    {
        flujoCajaRequest.Tipo = nuevoFiltro;
        FlujoCajaResultado flujoCajaResultado = ConsultarSubEncabezado(idCliente, flujoCajaRequest);
        bool isAlternateRow = false;
        foreach (var item in flujoCajaResultado.LineaFlujoCajas)
        {
            row = PintarLinea(idCliente, flujoCajaRequest, sheet, item, row, styles, isAlternateRow);
            isAlternateRow = !isAlternateRow;
        }
        flujoCajaRequest.Tipo = null;
    } 
    else
    {
        flujoCajaRequest.CodigoPosPre = nuevoFiltro;
        FlujoCajaResultado flujoCajaResultado = DetalleTercero(idCliente, flujoCajaRequest);
        foreach (var item in flujoCajaResultado.LineaFlujoCajaTerceros)
        {
            row = PintarLineaTercero(idCliente, sheet, item, row, styles);
        }
        flujoCajaRequest.CodigoPosPre = null;
    }
    return row;
}

private Dictionary<string, ICellStyle> CreateStyles(XSSFWorkbook workbook)
{
    Dictionary<string, ICellStyle> styles = new Dictionary<string, ICellStyle>();

    IFont titleFont = workbook.CreateFont();
    titleFont.FontHeightInPoints = 18;
    titleFont.IsBold = true;
    titleFont.Color = IndexedColors.White.Index;

    IFont subtitleFont = workbook.CreateFont();
    subtitleFont.FontHeightInPoints = 12;
    subtitleFont.IsItalic = true;

    IFont headerFont = workbook.CreateFont();
    headerFont.FontHeightInPoints = 11;
    headerFont.IsBold = true;
    headerFont.Color = IndexedColors.White.Index;

    IFont normalFont = workbook.CreateFont();
    normalFont.FontHeightInPoints = 10;

    IFont boldFont = workbook.CreateFont();
    boldFont.FontHeightInPoints = 10;
    boldFont.IsBold = true;

    IFont totalFont = workbook.CreateFont();
    totalFont.FontHeightInPoints = 11;
    totalFont.IsBold = true;
    totalFont.Color = IndexedColors.White.Index;

    IFont footerFont = workbook.CreateFont();
    footerFont.FontHeightInPoints = 9;
    footerFont.IsItalic = true;
    footerFont.Color = IndexedColors.Grey50Percent.Index;

    ICellStyle titleStyle = workbook.CreateCellStyle();
    titleStyle.SetFont(titleFont);
    titleStyle.Alignment = HorizontalAlignment.Center;
    titleStyle.VerticalAlignment = VerticalAlignment.Center;
    titleStyle.FillForegroundColor = IndexedColors.RoyalBlue.Index;
    titleStyle.FillPattern = FillPattern.SolidForeground;
    styles.Add("Title", titleStyle);

    ICellStyle subtitleStyle = workbook.CreateCellStyle();
    subtitleStyle.SetFont(subtitleFont);
    subtitleStyle.Alignment = HorizontalAlignment.Center;
    subtitleStyle.VerticalAlignment = VerticalAlignment.Center;
    styles.Add("Subtitle", subtitleStyle);

    ICellStyle columnHeader = workbook.CreateCellStyle();
    columnHeader.SetFont(headerFont);
    columnHeader.FillForegroundColor = IndexedColors.RoyalBlue.Index;
    columnHeader.FillPattern = FillPattern.SolidForeground;
    columnHeader.BorderBottom = BorderStyle.Medium;
    columnHeader.BorderLeft = BorderStyle.Thin;
    columnHeader.BorderRight = BorderStyle.Thin;
    columnHeader.BorderTop = BorderStyle.Medium;
    columnHeader.Alignment = HorizontalAlignment.Center;
    columnHeader.VerticalAlignment = VerticalAlignment.Center;
    columnHeader.TopBorderColor = IndexedColors.White.Index;
    columnHeader.BottomBorderColor = IndexedColors.White.Index;
    columnHeader.LeftBorderColor = IndexedColors.White.Index;
    columnHeader.RightBorderColor = IndexedColors.White.Index;
    styles.Add("ColumnHeader", columnHeader);

    ICellStyle normalRow = workbook.CreateCellStyle();
    normalRow.SetFont(normalFont);
    normalRow.BorderBottom = BorderStyle.Thin;
    normalRow.BorderLeft = BorderStyle.Thin;
    normalRow.BorderRight = BorderStyle.Thin;
    normalRow.BorderTop = BorderStyle.Thin;
    normalRow.VerticalAlignment = VerticalAlignment.Center;
    styles.Add("NormalRow", normalRow);

    ICellStyle alternateRow = workbook.CreateCellStyle();
    alternateRow.CloneStyleFrom(normalRow);
    alternateRow.FillForegroundColor = IndexedColors.Grey25Percent.Index;
    alternateRow.FillPattern = FillPattern.SolidForeground;
    styles.Add("AlternateRow", alternateRow);

    ICellStyle normalValue = workbook.CreateCellStyle();
    normalValue.SetFont(normalFont);
    normalValue.BorderBottom = BorderStyle.Thin;
    normalValue.BorderLeft = BorderStyle.Thin;
    normalValue.BorderRight = BorderStyle.Thin;
    normalValue.BorderTop = BorderStyle.Thin;
    normalValue.Alignment = HorizontalAlignment.Right;
    normalValue.VerticalAlignment = VerticalAlignment.Center;
    normalValue.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
    styles.Add("NormalValue", normalValue);

    ICellStyle alternateValue = workbook.CreateCellStyle();
    alternateValue.CloneStyleFrom(normalValue);
    alternateValue.FillForegroundColor = IndexedColors.Grey25Percent.Index;
    alternateValue.FillPattern = FillPattern.SolidForeground;
    styles.Add("AlternateValue", alternateValue);

    ICellStyle categoryRow = workbook.CreateCellStyle();
    categoryRow.SetFont(boldFont);
    categoryRow.BorderBottom = BorderStyle.Thin;
    categoryRow.BorderLeft = BorderStyle.Thin;
    categoryRow.BorderRight = BorderStyle.Thin;
    categoryRow.BorderTop = BorderStyle.Thin;
    categoryRow.FillForegroundColor = IndexedColors.LightBlue.Index;
    categoryRow.FillPattern = FillPattern.SolidForeground;
    categoryRow.VerticalAlignment = VerticalAlignment.Center;
    styles.Add("CategoryRow", categoryRow);

    ICellStyle categoryValue = workbook.CreateCellStyle();
    categoryValue.SetFont(boldFont);
    categoryValue.BorderBottom = BorderStyle.Thin;
    categoryValue.BorderLeft = BorderStyle.Thin;
    categoryValue.BorderRight = BorderStyle.Thin;
    categoryValue.BorderTop = BorderStyle.Thin;
    categoryValue.FillForegroundColor = IndexedColors.LightBlue.Index;
    categoryValue.FillPattern = FillPattern.SolidForeground;
    categoryValue.Alignment = HorizontalAlignment.Right;
    categoryValue.VerticalAlignment = VerticalAlignment.Center;
    categoryValue.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
    styles.Add("CategoryValue", categoryValue);

    ICellStyle totalRow = workbook.CreateCellStyle();
    totalRow.SetFont(totalFont);
    totalRow.BorderBottom = BorderStyle.Medium;
    totalRow.BorderLeft = BorderStyle.Thin;
    totalRow.BorderRight = BorderStyle.Thin;
    totalRow.BorderTop = BorderStyle.Medium;
    totalRow.FillForegroundColor = IndexedColors.RoyalBlue.Index;
    totalRow.FillPattern = FillPattern.SolidForeground;
    totalRow.VerticalAlignment = VerticalAlignment.Center;
    styles.Add("TotalRow", totalRow);

    ICellStyle totalValue = workbook.CreateCellStyle();
    totalValue.SetFont(totalFont);
    totalValue.BorderBottom = BorderStyle.Medium;
    totalValue.BorderLeft = BorderStyle.Thin;
    totalValue.BorderRight = BorderStyle.Thin;
    totalValue.BorderTop = BorderStyle.Medium;
    totalValue.FillForegroundColor = IndexedColors.RoyalBlue.Index;
    totalValue.FillPattern = FillPattern.SolidForeground;
    totalValue.Alignment = HorizontalAlignment.Right;
    totalValue.VerticalAlignment = VerticalAlignment.Center;
    totalValue.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
    styles.Add("TotalValue", totalValue);

    ICellStyle detailRow = workbook.CreateCellStyle();
    detailRow.SetFont(normalFont);
    detailRow.BorderBottom = BorderStyle.Thin;
    detailRow.BorderLeft = BorderStyle.Thin;
    detailRow.BorderRight = BorderStyle.Thin;
    detailRow.BorderTop = BorderStyle.Thin;
    detailRow.VerticalAlignment = VerticalAlignment.Center;
    detailRow.Indention = 3;
    styles.Add("DetailRow", detailRow);

    ICellStyle detailValue = workbook.CreateCellStyle();
    detailValue.CloneStyleFrom(normalValue);
    styles.Add("DetailValue", detailValue);

    ICellStyle footerStyle = workbook.CreateCellStyle();
    footerStyle.SetFont(footerFont);
    footerStyle.Alignment = HorizontalAlignment.Left;
    footerStyle.VerticalAlignment = VerticalAlignment.Center;
    styles.Add("Footer", footerStyle);

    ICellStyle signatureLabelStyle = workbook.CreateCellStyle();
    signatureLabelStyle.SetFont(boldFont);
    signatureLabelStyle.Alignment = HorizontalAlignment.Center;
    signatureLabelStyle.VerticalAlignment = VerticalAlignment.Bottom;
    signatureLabelStyle.BorderTop = BorderStyle.Thin;
    styles.Add("SignatureLabel", signatureLabelStyle);

    return styles;
}

    private string NombreTotal(String Language, string IdentificacionTercero)
    {
        if ("es".Equals(Language))
        {
            return IdentificacionTercero != null ? "Flujo Caja Neto" : "3. Saldo Disponible";
        }
        else
        {
            return IdentificacionTercero != null ? "Net Cash Flow" : "3. Available Balance";
        }
    }

    private string ObtenerEtiqueta(string Language)
    {
        return "es".Equals(Language) ? "Descripción" : "Description";
    }

    private List<LineaFlujoCaja> ProcesarMeses(List<string> meses, string tipo, List<LineaFlujoCaja> lineaFlujoCajas, string Language)
    {
        return meses.FindAll(mes => !mes.Equals(ObtenerEtiqueta(Language))).Select(mes => {
            var mesNum = _mesesService.ObtenerNumeroMes(mes, Language);
            return new LineaFlujoCaja() {
                Tipo = tipo,
                Mes = mesNum.HasValue ? 0 : mesNum.Value,
                Value = lineaFlujoCajas.FindAll(x => (x.Mes.ToString() == mes) || (_mesesService.ObtenerNombreMes(x.Mes, Language).Equals(mes))).Sum(x => x.Value)
            };
        }).ToList();
    }

}
