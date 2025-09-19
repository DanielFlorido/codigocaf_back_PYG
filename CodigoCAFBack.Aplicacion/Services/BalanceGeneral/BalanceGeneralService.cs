using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Aplicacion.Interfaces.Mail;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CodigoCAFBack.Aplicacion.Services.BalanceGeneral;

public class BalanceGeneralService : IBalanceGeneralService
{

    private readonly IBalanceGeneralRepository _balanceGeneralRepository;
    private readonly IClienteLogoService _clienteLogoService;
    private readonly IMesesService _mesesService;
    private readonly IMailService _mailService;
    private readonly IUsuarioMulticlienteService _usuarioMulticlienteService;

    public BalanceGeneralService(IBalanceGeneralRepository balanceGeneralRepository, IClienteLogoService clienteLogoService, IMesesService mesesService, IMailService mailService, IUsuarioMulticlienteService usuarioMulticlienteService)
    {
        _balanceGeneralRepository = balanceGeneralRepository;
        _clienteLogoService = clienteLogoService;
        _mesesService = mesesService;
        _mailService = mailService;
        _usuarioMulticlienteService = usuarioMulticlienteService;
    }

    public List<BalanceGeneralAnio> ListarAnioBalanceGeneral(Int32 idCliente)
    {
        return _balanceGeneralRepository.ListarAnioBalanceGeneral(idCliente);
    }

    public BalanceGeneralResultado ConsultarEncabezado(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        List<LineaBalanceGeneral> encabezados = _balanceGeneralRepository.BuscarEncabezado(idCliente, balanceGeneralRequest);

        BalanceGeneralResultado balanceGeneralResultado = new BalanceGeneralResultado();
        List<string> etiquetas = new List<string>();
        foreach (var item in encabezados.GroupBy(x => x.Mes))
        {
            etiquetas.Add(item.Key.ToString());
        }
        
        balanceGeneralResultado.Data = new List<LineaBalanceGeneral>();
        foreach (var item in encabezados.GroupBy(x => x.Etiqueta))
        {
            balanceGeneralResultado.Data.Add(new LineaBalanceGeneral()
            {
                Etiqueta = item.Key,
                Texto = item.ToList().First().Texto,
                Data = procesar(etiquetas, item.ToList(), balanceGeneralRequest.Language, true)
            });
        }
        if (balanceGeneralRequest.Month.HasValue && balanceGeneralRequest.Month.Value >= 1)
        {
            foreach (var item in balanceGeneralResultado.Data)
            {
                item.Data.RemoveAll(removed => removed.Mes != balanceGeneralRequest.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
        }
        balanceGeneralResultado.Data.Add(new LineaBalanceGeneral()
        {
            Etiqueta = "",
            Texto = balanceGeneralRequest.Language.Equals("es") ? "Total general" : "Grand total",
            Data = procesarTotalGeneral(etiquetas, balanceGeneralResultado.Data)
        });
        balanceGeneralResultado.Etiquetas = traducccionEtiquetas(balanceGeneralRequest.Language, etiquetas, balanceGeneralRequest.ViewAccountCode);
        balanceGeneralResultado.UtilidadMes = procesarUtilidadMes(etiquetas, balanceGeneralResultado.Data, balanceGeneralRequest.Language);
        balanceGeneralResultado.UtilidadAcumulada = procesarUtilidadAcumulada(balanceGeneralResultado.UtilidadMes, balanceGeneralRequest.Language);
        return balanceGeneralResultado;
    }

    public BalanceGeneralResultado ConsultarSubEncabezado(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {        
        List<LineaBalanceGeneral> encabezados = _balanceGeneralRepository.BuscarSubEncabezado(idCliente, balanceGeneralRequest);

        BalanceGeneralResultado balanceGeneralResultado = new BalanceGeneralResultado();
        balanceGeneralResultado.Data = new List<LineaBalanceGeneral>();
        foreach (var item in encabezados.GroupBy(x => x.Etiqueta))
        {
            balanceGeneralResultado.Data.Add(new LineaBalanceGeneral()
            {
                Etiqueta = item.Key,
                Texto = item.ToList().FirstOrDefault().Texto,
                Data = procesar(balanceGeneralRequest.etiquetas, item.ToList(), balanceGeneralRequest.Language, false)
            });
        }
        if (balanceGeneralRequest.Month.HasValue && balanceGeneralRequest.Month.Value >= 1)
        {
            foreach (var item in balanceGeneralResultado.Data)
            {
                item.Data.RemoveAll(removed => removed.Mes != balanceGeneralRequest.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
        }
        return balanceGeneralResultado;
    }

    public BalanceGeneralResultado ConsultarDetalle(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {       
        List<LineaBalanceGeneral> encabezados = _balanceGeneralRepository.BuscarDetalle(idCliente, balanceGeneralRequest);
        List<GrupoClasificacionCorriente> corrientes = _balanceGeneralRepository.BuscarClasificacionCorriente(idCliente, balanceGeneralRequest, true);
        var prefix = balanceGeneralRequest.Prefix == null ? "" : balanceGeneralRequest.Prefix;
        BalanceGeneralResultado balanceGenerales = new BalanceGeneralResultado();
        balanceGenerales.CurrencyData = new List<LineaBalanceGeneral>();
        balanceGenerales.NotCurrencyData = new List<LineaBalanceGeneral>();
        balanceGenerales.CurrencyTotal = new LineaBalanceGeneral() { Data = new List<LineaBalanceGeneral>(), Texto = prefix + " " + (balanceGeneralRequest.Language == "es" ? " Corriente" : " Currency") };
        balanceGenerales.NotCurrencyTotal = new LineaBalanceGeneral() { Data = new List<LineaBalanceGeneral>(), Texto = prefix + " " + (balanceGeneralRequest.Language == "es" ? "No Corriente" : "Not Currency") };
        foreach (var item in encabezados.GroupBy(x => x.Etiqueta))
        {
            var dataLine = procesar(balanceGeneralRequest.etiquetas, item.ToList(), balanceGeneralRequest.Language, false);
            if (corrientes.Any(x => x.CodigoPosPre == item.Key)) { 
                balanceGenerales.CurrencyData.Add(new LineaBalanceGeneral()
                {
                    Etiqueta = item.Key,
                    Texto = item.ToList().First().Texto,
                    Data = dataLine
                });
                for (var i = 0; i < dataLine.Count; i++)
                {
                    if (balanceGenerales.CurrencyTotal.Data.Count == i) { 
                        balanceGenerales.CurrencyTotal.Data.Add(new LineaBalanceGeneral()
                        {
                            Mes = dataLine[i].Mes,
                            Total = dataLine[i].Total
                        });
                    } else
                    {
                        balanceGenerales.CurrencyTotal.Data[i].Total += dataLine[i].Total;
                    }
                }
            } else
            {
                balanceGenerales.NotCurrencyData.Add(new LineaBalanceGeneral()
                {
                    Etiqueta = item.Key,
                    Texto = item.ToList().First().Texto,
                    Data = dataLine
                });
                for (var i = 0; i < dataLine.Count; i++)
                {
                    if (balanceGenerales.NotCurrencyTotal.Data.Count == i)
                    {
                        balanceGenerales.NotCurrencyTotal.Data.Add(new LineaBalanceGeneral()
                        {
                            Mes = dataLine[i].Mes,
                            Total = dataLine[i].Total
                        });
                    }
                    else
                    {
                        balanceGenerales.NotCurrencyTotal.Data[i].Total += dataLine[i].Total;
                    }
                }
            }
        }
        if (balanceGeneralRequest.Month.HasValue && balanceGeneralRequest.Month.Value >= 1)
        {
            foreach (var item in balanceGenerales.CurrencyData)
            {
                item.Data.RemoveAll(removed => removed.Mes != balanceGeneralRequest.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
            balanceGenerales.CurrencyTotal.Data.RemoveAll(removed => removed.Mes != balanceGeneralRequest.Month.Value);
            balanceGenerales.CurrencyTotal.Data.Add(new LineaBalanceGeneral()
            {
                Total = balanceGenerales.CurrencyTotal.Data.Sum(x => x.Total)
            });
            foreach (var item in balanceGenerales.NotCurrencyData)
            {
                item.Data.RemoveAll(removed => removed.Mes != balanceGeneralRequest.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
            balanceGenerales.NotCurrencyTotal.Data.RemoveAll(removed => removed.Mes != balanceGeneralRequest.Month.Value);
            balanceGenerales.NotCurrencyTotal.Data.Add(new LineaBalanceGeneral()
            {
                Total = balanceGenerales.NotCurrencyTotal.Data.Sum(x => x.Total)
            });
        }
        return balanceGenerales;
    }

    public BalanceGeneralResultado ConsultarTercero(Int32 IdCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        List<LineaBalanceGeneral> encabezados = _balanceGeneralRepository.BuscarTerceros(IdCliente, balanceGeneralRequest);

        BalanceGeneralResultado balanceGenerales = new BalanceGeneralResultado();
        balanceGenerales.Data = new List<LineaBalanceGeneral>();
        foreach (var item in encabezados.OrderBy(x => x.Etiqueta).GroupBy(x => x.Etiqueta))
        {
            balanceGenerales.Data.Add(new LineaBalanceGeneral()
            {
                Etiqueta = item.Key,
                Texto = item.ToList().First().Texto,
                Data = procesar(balanceGeneralRequest.etiquetas, item.ToList(), balanceGeneralRequest.Language, false)
            });
        }
        if (balanceGeneralRequest.Month.HasValue && balanceGeneralRequest.Month.Value >= 1)
        {
            foreach (var item in balanceGenerales.Data)
            {
                item.Data.RemoveAll(removed => removed.Mes != balanceGeneralRequest.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
        }
        return balanceGenerales;
    }

    public void EnviarExcel(Int32 idCliente, BalanceGeneralRequest request)
    {
        _mailService.SendEmailAsync(_usuarioMulticlienteService.ObtenerCorreoParaCliente(idCliente), TituloCorreo(request.Language), CuerpoCorreo(request.Language), new List<byte[]>() { this.GenerarExcel(idCliente, request) }
            , new List<string>() { "BalanceGeneral.xlsx" }, false);
    }

    public void EnviarExcelAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        _mailService.SendEmailAsync(_usuarioMulticlienteService.ObtenerCorreoParaCliente(idCliente), TituloCorreo(balanceGeneralAnioRequest.Language), CuerpoCorreo(balanceGeneralAnioRequest.Language), new List<byte[]>() { this.GenerarExcelAgrupado(idCliente, balanceGeneralAnioRequest) }
            , new List<string>() { "BalanceGeneral.xlsx" }, false);
    }

    private string TituloCorreo(string Lenguaje)
    {
        return "es".Equals(Lenguaje) ? "Reporte Balance General" : "Balance Sheet Report";

    }

    private string CuerpoCorreo(string Lenguaje)
    {
        return "es".Equals(Lenguaje) ? "Hola, adjunto encontraras el reporte de balance general" : "Hello, attached you will find the balance sheet report.";
    }

    public byte[] GenerarExcelAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        balanceGeneralAnioRequest.Type = null;
        balanceGeneralAnioRequest.Group = null;
        balanceGeneralAnioRequest.CodigoPosPre = null;
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Balance General");

        sheet.DisplayGridlines = false;
        sheet.IsPrintGridlines = false;
        sheet.DefaultRowHeight = 400;

        var logoClienteRequest = new LogoClienteRequest
        {
            IdCliente = idCliente,
            IdUsuario = balanceGeneralAnioRequest.UserId
        };

        var logoCliente = _clienteLogoService.BuscarLogoCliente(logoClienteRequest);

        BalanceGeneralResultadoGrupo balanceGeneralResultadoGrupo = ConsultarEncabezadoAgrupado(idCliente, balanceGeneralAnioRequest);

        sheet.SetColumnWidth(0, 6000);
        sheet.SetColumnWidth(1, 4000);
        sheet.SetColumnWidth(2, 5000);
        sheet.SetColumnWidth(3, 5000);
        sheet.SetColumnWidth(4, 5000);
        sheet.SetColumnWidth(5, 4000);

        var rowTitle = sheet.CreateRow(0);
        rowTitle.Height = 1200;
        var titleCell = rowTitle.CreateCell(0);
        titleCell.SetCellValue(balanceGeneralAnioRequest.Language == "es" ? "Estado Situación Financiera" : "Statement of Financial Position");

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

        var subtitleRow = sheet.CreateRow(1);
        subtitleRow.Height = 500;
        var subtitleCell = subtitleRow.CreateCell(0);
        subtitleCell.SetCellValue($"Comparativo {balanceGeneralAnioRequest.Year1} - {balanceGeneralAnioRequest.Year2}");

        var subtitleFont = workbook.CreateFont();
        subtitleFont.FontHeightInPoints = 12;
        subtitleFont.IsItalic = true;

        var subtitleStyle = workbook.CreateCellStyle();
        subtitleStyle.SetFont(subtitleFont);
        subtitleStyle.Alignment = HorizontalAlignment.Center;

        subtitleCell.CellStyle = subtitleStyle;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 5));

        var headerRow = sheet.CreateRow(4);
        headerRow.Height = 600;

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

        var cliente = _usuarioMulticlienteService.ConsultarClientesPorUsuario(balanceGeneralAnioRequest.UserId).Find(client => client.IDCLIENTE == idCliente);
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
        cellClient.SetCellValue(balanceGeneralAnioRequest.Year1.ToString());
        cellClient.CellStyle = headerStyle;
        cellClient = rowClient.CreateCell(3);
        sheet.SetColumnWidth(0, 4000);
        cellClient.SetCellValue(balanceGeneralAnioRequest.Year2.ToString());
        cellClient.CellStyle = headerStyle;
        var cellIndex = 0;
        foreach (var item in balanceGeneralResultadoGrupo.Etiquetas)
        {
            var cell = headerRow.CreateCell(cellIndex);
            cell.SetCellValue(item);
            cell.CellStyle = headerStyle;
            cellIndex++;
        }

        PintarBalanceGrupo(sheet, 5, balanceGeneralResultadoGrupo, idCliente, balanceGeneralAnioRequest);

        int lastRow = sheet.LastRowNum + 2;
        var footerRow = sheet.CreateRow(lastRow);
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

        if (balanceGeneralAnioRequest.IncludeSignature && logoCliente != null)
        {
            int signatureRow = lastRow + 1;
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
                contadorLabel.SetCellValue(balanceGeneralAnioRequest.Language.Equals("es") ? 
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
                revisorLabel.SetCellValue(balanceGeneralAnioRequest.Language.Equals("es") ? 
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
                gerenteLabel.SetCellValue(balanceGeneralAnioRequest.Language.Equals("es") ? 
                    "Gerente" : "Manager");
                gerenteLabel.CellStyle = footerStyle;
            }
            
            lastRow = signatureRow + 3;
        }
        var noticeRow = sheet.CreateRow(lastRow + 1);
        var noticeCell = noticeRow.CreateCell(0);
        noticeCell.SetCellValue("Documento confidencial. Prohibida su reproducción parcial o total sin autorización.");
        noticeCell.CellStyle = footerStyle;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(lastRow + 1, lastRow + 1, 0, 5));

        var stream = new MemoryStream();
        workbook.Write(stream);
        var content = stream.ToArray();

        return content;
    }

    private Int32 PintarBalanceGrupo(ISheet sheet, int rowNumber, BalanceGeneralResultadoGrupo resultData, Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {

        IWorkbook workbook = sheet.Workbook;

        var textStyle = workbook.CreateCellStyle();
        textStyle.Alignment = HorizontalAlignment.Left;
        textStyle.BorderTop = BorderStyle.Thin;
        textStyle.BorderBottom = BorderStyle.Thin;
        textStyle.BorderLeft = BorderStyle.Thin;
        textStyle.BorderRight = BorderStyle.Thin;

        var altTextStyle = workbook.CreateCellStyle();
        altTextStyle.CloneStyleFrom(textStyle);
        altTextStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
        altTextStyle.FillPattern = FillPattern.SolidForeground;

        var numberStyle = workbook.CreateCellStyle();
        numberStyle.Alignment = HorizontalAlignment.Right;
        numberStyle.BorderTop = BorderStyle.Thin;
        numberStyle.BorderBottom = BorderStyle.Thin;
        numberStyle.BorderLeft = BorderStyle.Thin;
        numberStyle.BorderRight = BorderStyle.Thin;
        numberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

        var altNumberStyle = workbook.CreateCellStyle();
        altNumberStyle.CloneStyleFrom(numberStyle);
        altNumberStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
        altNumberStyle.FillPattern = FillPattern.SolidForeground;

        var positiveStyle = workbook.CreateCellStyle();
        positiveStyle.CloneStyleFrom(numberStyle);
        var positiveFont = workbook.CreateFont();
        positiveFont.Color = IndexedColors.DarkGreen.Index;
        positiveStyle.SetFont(positiveFont);

        var negativeStyle = workbook.CreateCellStyle();
        negativeStyle.CloneStyleFrom(numberStyle);
        var negativeFont = workbook.CreateFont();
        negativeFont.Color = IndexedColors.DarkRed.Index;
        negativeStyle.SetFont(negativeFont);

        var parentFont = workbook.CreateFont();
        parentFont.IsBold = true;
        var parentStyle = workbook.CreateCellStyle();
        parentStyle.CloneStyleFrom(textStyle);
        parentStyle.SetFont(parentFont);

        bool isAlternateRow = false;

        var idProcess = 0;
        List<LineaBalanceGeneralAgrupado> Data = null;

        do
        {
            if (balanceGeneralAnioRequest.Type == null && idProcess == 0)
            {
                Data = resultData.Data;
                idProcess = 1;
            }
            else if (balanceGeneralAnioRequest.Type != null && idProcess < 4)
            {
                if (idProcess == 1)
                {
                    Data = resultData.CurrencyData;
                    idProcess = 2;
                }
                else if (idProcess == 3)
                {
                    Data = resultData.NotCurrencyData;
                    idProcess = 4;
                }
                else if (resultData.CurrencyData.Count > 1 && idProcess == 0)
                {
                    Data = new List<LineaBalanceGeneralAgrupado>() {
                        resultData.CurrencyTotal
                    };
                    idProcess = 1;
                }
                else if (resultData.NotCurrencyData.Count > 1 && (idProcess == 0 || idProcess == 2))
                {
                    Data = new List<LineaBalanceGeneralAgrupado>() {
                        resultData.NotCurrencyTotal
                    };
                    idProcess = 3;
                }
                else
                {
                    Data = null;
                }
            }
            else
            {
                Data = null;
            }

            foreach (var item in Data)
            {
                var row = sheet.CreateRow(rowNumber);

                ICellStyle currentTextStyle = isAlternateRow ? altTextStyle : textStyle;
                ICellStyle currentNumberStyle = isAlternateRow ? altNumberStyle : numberStyle;

                var cell0 = row.CreateCell(0);
                cell0.SetCellValue(item.Texto);
                cell0.CellStyle = balanceGeneralAnioRequest.Type == null ? parentStyle : currentTextStyle;
                if (balanceGeneralAnioRequest.CodigoPosPre != null)
                {
                    cell0.SetCellValue("      " + item.Texto);
                }
                else if (balanceGeneralAnioRequest.Group != null)
                {
                    cell0.SetCellValue("    " + item.Texto);
                }
                else if (balanceGeneralAnioRequest.Type != null)
                {
                    cell0.SetCellValue("  " + item.Texto);
                }

                var indexCell = 1;
                if (balanceGeneralAnioRequest.ViewAccountCode)
                {
                    var codeAccountCell = row.CreateCell(indexCell);
                    codeAccountCell.SetCellValue(item.Codigo.ToString());
                    codeAccountCell.CellStyle = currentTextStyle;
                    indexCell++;
                }

                var cell1 = row.CreateCell(indexCell);
                cell1.SetCellValue(item.Notas.HasValue ? item.Notas.Value.ToString() : "");
                cell1.CellStyle = currentTextStyle;
                indexCell++;

                var cell2 = row.CreateCell(indexCell);
                cell2.SetCellValue(Convert.ToDouble(item.ValorPrevio));
                cell2.CellStyle = currentNumberStyle;
                indexCell++;

                var cell3 = row.CreateCell(indexCell);
                cell3.SetCellValue(Convert.ToDouble(item.ValorActual));
                cell3.CellStyle = currentNumberStyle;
                indexCell++;

                var cell4 = row.CreateCell(indexCell);
                cell4.SetCellValue(Convert.ToDouble(item.USDVariacion));

                if (item.USDVariacion > 0)
                    cell4.CellStyle = positiveStyle;
                else if (item.USDVariacion < 0)
                    cell4.CellStyle = negativeStyle;
                else
                    cell4.CellStyle = currentNumberStyle;
                indexCell++;

                var cell5 = row.CreateCell(indexCell);
                cell5.SetCellValue(Convert.ToDouble(item.ValorVariacion));

                if (item.ValorVariacion > 0)
                    cell5.CellStyle = positiveStyle;
                else if (item.ValorVariacion < 0)
                    cell5.CellStyle = negativeStyle;
                else
                    cell5.CellStyle = currentNumberStyle;
                indexCell++;

                rowNumber++;
                isAlternateRow = !isAlternateRow;

                if (balanceGeneralAnioRequest.Type == null)
                {
                    balanceGeneralAnioRequest.Type = item.Codigo;
                    rowNumber = PintarBalanceGrupo(sheet, rowNumber, ConsultarSubEncabezadoAgrupado(idCliente, balanceGeneralAnioRequest), idCliente, balanceGeneralAnioRequest);
                    balanceGeneralAnioRequest.Type = null;
                }
                else if (balanceGeneralAnioRequest.Group == null)
                {
                    balanceGeneralAnioRequest.Group = item.Codigo;
                    rowNumber = PintarBalanceGrupo(sheet, rowNumber, ConsultarDetalleAgrupado(idCliente, balanceGeneralAnioRequest), idCliente, balanceGeneralAnioRequest);
                    balanceGeneralAnioRequest.Group = null;
                }
                else if (balanceGeneralAnioRequest.CodigoPosPre == null && idProcess % 2 == 0)
                {
                    balanceGeneralAnioRequest.CodigoPosPre = item.Codigo;
                    rowNumber = PintarBalanceGrupo(sheet, rowNumber, ConsultarTerceroAgrupado(idCliente, balanceGeneralAnioRequest), idCliente, balanceGeneralAnioRequest);
                    balanceGeneralAnioRequest.CodigoPosPre = null;
                }
            }
        } while (Data != null);

        return rowNumber;
    }

    public byte[] GenerarExcel(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        balanceGeneralRequest.Type = null;
        balanceGeneralRequest.Grupo = null;
        balanceGeneralRequest.CodigoPosPre = null;
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Balance General");
        
        sheet.IsPrintGridlines = false;
        sheet.DisplayGridlines = false;
        sheet.DefaultRowHeight = 400;
        
        var logoClienteRequest = new LogoClienteRequest
        {
            IdCliente = idCliente,
            IdUsuario = balanceGeneralRequest.UserId,
        };

        var logoCliente = _clienteLogoService.BuscarLogoCliente(logoClienteRequest);

        sheet.SetColumnWidth(0, 6000);
        sheet.SetColumnWidth(1, 4000);
        sheet.SetColumnWidth(2, 5000);
        sheet.SetColumnWidth(3, 5000);
        sheet.SetColumnWidth(4, 5000);
        sheet.SetColumnWidth(5, 4000);

        var rowTitle = sheet.CreateRow(0);
        rowTitle.Height = 1200;
        var titleCell = rowTitle.CreateCell(0);
        titleCell.SetCellValue("Balance General Comparativo");

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

        BalanceGeneralResultado resultado = ConsultarEncabezado(idCliente, balanceGeneralRequest);
        var cliente = _usuarioMulticlienteService.ConsultarClientesPorUsuario(balanceGeneralRequest.UserId).Find(client => client.IDCLIENTE == idCliente);
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
        cellClient.SetCellValue(balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value.ToString() : (balanceGeneralRequest.DateFrom!= null ? balanceGeneralRequest.DateFrom : ""));
        cellClient.CellStyle = headerStyle;
        cellClient = rowClient.CreateCell(3);
        sheet.SetColumnWidth(0, 4000);
        var month = balanceGeneralRequest.Year.HasValue ? 
            balanceGeneralRequest.Month.HasValue ? _mesesService.ConsultarMes(balanceGeneralRequest.Language, balanceGeneralRequest.Month.Value) : "es".Equals(balanceGeneralRequest.Language) ? "Todos Los Meses" : "Every Month" : "";
        cellClient.SetCellValue(month.Length == 0 ? (balanceGeneralRequest.DateTo != null ? balanceGeneralRequest.DateTo : "") : month);
        cellClient.CellStyle = headerStyle;
        var colIndex = 0;
        foreach (var property in resultado.Etiquetas)
        {
            var cell = rowHeader.CreateCell(colIndex);
            if(colIndex == 0)
                sheet.SetColumnWidth(colIndex, 12000);
            else if(resultado.Etiquetas.Count -1 == colIndex)
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
                item.Texto.Contains("Total") ? totalStyle : (isAlternateRow ? altRowStyle : dataCellStyle));
            
            isAlternateRow = !isAlternateRow;
            row++;
        }
        row--;
        var rowData = sheet.CreateRow(row);
        var labelCell = rowData.CreateCell(0);
        labelCell.SetCellValue(resultado.UtilidadMes.Texto);
        labelCell.CellStyle = totalLabelStyle;

        colIndex = 1;
        foreach (var data in resultado.UtilidadMes.Data)
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
        labelCell.SetCellValue(resultado.UtilidadAcumulada.Texto);
        labelCell.CellStyle = totalLabelStyle;

        colIndex = 1;
        foreach (var data in resultado.UtilidadAcumulada.Data)
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

    private int VisualizarLineaBalanceGeneral(Int32 IdCliente, BalanceGeneralRequest balanceGeneralRequest, 
        LineaBalanceGeneral lineaBalanceGeneral, IWorkbook workbook, ISheet sheet, int row,
        ICellStyle labelStyle, ICellStyle dataStyle, ICellStyle mainLabelStyle, ICellStyle mainDataStyle)
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
            row = ProcesarRespuetas(IdCliente, balanceGeneralRequest, lineaBalanceGeneral.Etiqueta, workbook, sheet, row, labelStyle, dataStyle);
        }
        return row;
    }

    private int ProcesarRespuetas(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest, string newFilter,
        IWorkbook workbook, ISheet sheet, int row, ICellStyle labelStyle, ICellStyle dataStyle)
    {
        BalanceGeneralResultado balanceGeneralResultado = new BalanceGeneralResultado();
        if (balanceGeneralRequest.Type != null)
        {
            balanceGeneralResultado = ConsultarSubEncabezado(idCliente, balanceGeneralRequest);
        }
        else if (balanceGeneralRequest.Grupo != null)
        {
            balanceGeneralResultado = ConsultarDetalle(idCliente, balanceGeneralRequest);
        }
        else if (balanceGeneralRequest.CodigoPosPre != null)
        {
            balanceGeneralResultado = ConsultarTercero(idCliente, balanceGeneralRequest);
        }
        else
        {
            balanceGeneralResultado.Data = new List<LineaBalanceGeneral>();
        }

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
        if (balanceGeneralRequest.Grupo != null) {
            personCellStyle = workbook.CreateCellStyle();
            personCellStyle.CloneStyleFrom(indentLabelStyle);
            personCellStyle.Indention = 4;
            var personFont = workbook.CreateFont();
            personFont.Underline = FontUnderlineType.Single;
            personCellStyle.SetFont(personFont);
        } else if (balanceGeneralRequest.CodigoPosPre != null)
        {
            personCellStyle = workbook.CreateCellStyle();
            personCellStyle.CloneStyleFrom(indentLabelStyle);
            personCellStyle.Indention = 5;
            var personFont = workbook.CreateFont();
            personFont.IsItalic = true;
            personCellStyle.SetFont(personFont);
        }

        bool isAlternateRow = false;
        foreach (var linea in balanceGeneralResultado.Data)
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
                                                    workbook, sheet, row, labelStyle, dataStyle);

            isAlternateRow = !isAlternateRow;
        }
        return row;
    }

    private List<string> BuscarEtiquetas(BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        return new List<string>{ balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2 };
    }

    public BalanceGeneralResultadoGrupo ConsultarEncabezadoAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        List<LineaBalanceGeneralAnio> lineaBalanceGeneralAnios = _balanceGeneralRepository.BuscarEncabezadoAnio(idCliente, balanceGeneralAnioRequest);
        BalanceGeneralResultadoGrupo balanceGeneralEncabezadoGrupo = new BalanceGeneralResultadoGrupo();
        if(balanceGeneralAnioRequest.ViewAccountCode) {
            balanceGeneralEncabezadoGrupo.Etiquetas = "es".Equals(balanceGeneralAnioRequest.Language) ? new List<string> { "Descripción", "Código Cuenta", "Nota", balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, "Variación $", "Variación %" } :
                                                                                                new List<string> { "Description", "Account Code", "Note", balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, "Variation $", "Variation %" };
        } else {
            balanceGeneralEncabezadoGrupo.Etiquetas = "es".Equals(balanceGeneralAnioRequest.Language) ? new List<string> { "Descripción", "Nota", balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, "Variación $", "Variación %" } :
                                                                                                    new List<string> { "Description", "Note", balanceGeneralAnioRequest.Year1, balanceGeneralAnioRequest.Year2, "Variation $", "Variation %" };
        }
        balanceGeneralEncabezadoGrupo.Data = new List<LineaBalanceGeneralAgrupado>();

        foreach (var item in lineaBalanceGeneralAnios.GroupBy(x => x.Codigo))
        {
            Nullable<decimal> total1 = null;
            Nullable<decimal> total2 = null;
            foreach (var item1 in BuscarEtiquetas(balanceGeneralAnioRequest))
            {
                LineaBalanceGeneralAnio resultadoBuscado = item.ToList().Find(x => x.Year == Int32.Parse(item1));
                if (!total1.HasValue)
                {
                    total1 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
                else if (!total2.HasValue)
                {
                    total2 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
            }
            decimal previo = total1.HasValue ? total1.Value : 0;
            decimal actual = total2.HasValue ? total2.Value : 0;
            balanceGeneralEncabezadoGrupo.Data.Add(new LineaBalanceGeneralAgrupado
            {
                Codigo = item.Key,
                Texto = item.First().Texto,
                Notas = item.First().Notas,
                ValorPrevio = previo,
                ValorActual = actual,
                ValorVariacion = actual - previo,
                ValorPorcentaje = previo == 0 ? 100 : ((actual / previo) - 1)
            });
        }
        return balanceGeneralEncabezadoGrupo;
    }

    public BalanceGeneralResultadoGrupo ConsultarSubEncabezadoAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        List<LineaBalanceGeneralAnio> lineaBalanceGeneralAnios = _balanceGeneralRepository.BuscarSubEncabezadoAnio(idCliente, balanceGeneralAnioRequest);
        BalanceGeneralResultadoGrupo balanceGeneralResultadoGrupo = new BalanceGeneralResultadoGrupo();
        balanceGeneralResultadoGrupo.Data = new List<LineaBalanceGeneralAgrupado>();
        
        foreach (var item in lineaBalanceGeneralAnios.GroupBy(x => x.Codigo))
        {
            Nullable<decimal> total1 = null;
            Nullable<decimal> total2 = null;
            foreach (var item1 in BuscarEtiquetas(balanceGeneralAnioRequest))
            {
                LineaBalanceGeneralAnio resultadoBuscado = item.ToList().Find(x => x.Year == Int32.Parse(item1));
                if (!total1.HasValue)
                {
                    total1 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
                else if (!total2.HasValue)
                {
                    total2 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
            }
            decimal previo = total1.HasValue ? total1.Value : 0;
            decimal actual = total2.HasValue ? total2.Value : 0;
            balanceGeneralResultadoGrupo.Data.Add(new LineaBalanceGeneralAgrupado
            {
                Codigo = item.Key,
                Notas = item.First().Notas,
                Texto = item.First().Texto,
                ValorPrevio = previo,
                ValorActual = actual,
                ValorVariacion = actual - previo,
                ValorPorcentaje = previo == 0 ? 100 : ((actual / previo) - 1)
            });
        }
        return balanceGeneralResultadoGrupo;
    }

    public BalanceGeneralResultadoGrupo ConsultarDetalleAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        List<LineaBalanceGeneralAnio> lineaBalanceGeneralAnios = _balanceGeneralRepository.BuscarDetalleAnio(idCliente, balanceGeneralAnioRequest);

        var prefix = balanceGeneralAnioRequest.Prefix == null ? "" : balanceGeneralAnioRequest.Prefix;
        BalanceGeneralResultadoGrupo balanceGeneralResultadoGrupo = new BalanceGeneralResultadoGrupo();
        balanceGeneralResultadoGrupo.CurrencyData = new List<LineaBalanceGeneralAgrupado>();
        balanceGeneralResultadoGrupo.NotCurrencyData = new List<LineaBalanceGeneralAgrupado>();
        balanceGeneralResultadoGrupo.CurrencyTotal = new LineaBalanceGeneralAgrupado()
        {
            Codigo = null,
            Texto = prefix + " " + ("es".Equals(balanceGeneralAnioRequest.Language) ? "Corriente" : "Currency"),
            Notas = null,
            ValorPrevio = 0,
            ValorActual = 0,
        };
        balanceGeneralResultadoGrupo.NotCurrencyTotal = new LineaBalanceGeneralAgrupado()
        {
            Codigo = null,
            Texto = prefix + " " + ("es".Equals(balanceGeneralAnioRequest.Language) ? "No Corriente" : "Not Currency"),
            Notas = null,
            ValorPrevio = 0,
            ValorActual = 0,
        };
        List<GrupoClasificacionCorriente> grupoClasificacionCorrientes = _balanceGeneralRepository.BuscarGrupoClasificacionCorriente(idCliente, balanceGeneralAnioRequest, true);

        foreach (var item in lineaBalanceGeneralAnios.GroupBy(x => x.Codigo))
        {
            Nullable<decimal> total1 = null;
            Nullable<decimal> total2 = null;
            foreach (var item1 in BuscarEtiquetas(balanceGeneralAnioRequest))
            {
                LineaBalanceGeneralAnio resultadoBuscado = item.ToList().Find(x => x.Year == Int32.Parse(item1));
                if (!total1.HasValue)
                {
                    total1 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
                else if (!total2.HasValue)
                {
                    total2 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
            }
            decimal previo = total1.HasValue ? total1.Value : 0;
            decimal actual = total2.HasValue ? total2.Value : 0;
            if (grupoClasificacionCorrientes.Any(value => value.CodigoPosPre == item.Key))
            {
                balanceGeneralResultadoGrupo.CurrencyData.Add(new LineaBalanceGeneralAgrupado
                {
                    Codigo = item.Key,
                    Texto = item.First().Texto,
                    Notas = item.First().Notas,
                    ValorPrevio = previo,
                    ValorActual = actual,
                    ValorVariacion = actual - previo,
                    ValorPorcentaje = previo == 0 ? 100 : ((actual / previo) - 1)
                });
                balanceGeneralResultadoGrupo.CurrencyTotal.ValorPrevio += previo;
                balanceGeneralResultadoGrupo.CurrencyTotal.ValorActual += actual;
            } else {
                balanceGeneralResultadoGrupo.NotCurrencyData.Add(new LineaBalanceGeneralAgrupado
                {
                    Codigo = item.Key,
                    Texto = item.First().Texto,
                    Notas = item.First().Notas,
                    ValorPrevio = previo,
                    ValorActual = actual,
                    ValorVariacion = actual - previo,
                    ValorPorcentaje = previo == 0 ? 100 : ((actual / previo) - 1)
                });
                balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorPrevio += previo;
                balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorActual += actual;
            }
        }
        balanceGeneralResultadoGrupo.CurrencyTotal.ValorVariacion = balanceGeneralResultadoGrupo.CurrencyTotal.ValorActual - balanceGeneralResultadoGrupo.CurrencyTotal.ValorPrevio;
        balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorVariacion = balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorActual - balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorPrevio;
        balanceGeneralResultadoGrupo.CurrencyTotal.ValorPorcentaje = balanceGeneralResultadoGrupo.CurrencyTotal.ValorPrevio == 0 ? 100 : ((balanceGeneralResultadoGrupo.CurrencyTotal.ValorActual / balanceGeneralResultadoGrupo.CurrencyTotal.ValorPrevio) - 1);
        balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorPorcentaje = balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorPrevio == 0 ? 100 : ((balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorActual / balanceGeneralResultadoGrupo.NotCurrencyTotal.ValorPrevio) - 1);
        return balanceGeneralResultadoGrupo;
    }

    public BalanceGeneralResultadoGrupo ConsultarTerceroAgrupado(Int32 idCliente, BalanceGeneralAnioRequest balanceGeneralAnioRequest)
    {
        List<LineaBalanceGeneralAnioDet> lineaBalanceGeneralAnios = _balanceGeneralRepository.BuscarTerceroAnio(idCliente, balanceGeneralAnioRequest);

        BalanceGeneralResultadoGrupo balanceGeneralResultadoGrupo = new BalanceGeneralResultadoGrupo();
        balanceGeneralResultadoGrupo.Data = new List<LineaBalanceGeneralAgrupado>();
        foreach (var item in lineaBalanceGeneralAnios.GroupBy(x => x.Codigo))
        {
            Nullable<decimal> total1 = null;
            Nullable<decimal> total2 = null;
            foreach (var item1 in BuscarEtiquetas(balanceGeneralAnioRequest))
            {
                LineaBalanceGeneralAnioDet resultadoBuscado = item.ToList().Find(x => x.Year == Int32.Parse(item1));
                if (!total1.HasValue)
                {
                    total1 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
                else if (!total2.HasValue)
                {
                    total2 = resultadoBuscado != null ? resultadoBuscado.Total : 0;
                }
            }
            decimal previo = total1.HasValue ? total1.Value : 0;
            decimal actual = total2.HasValue ? total2.Value : 0;
            balanceGeneralResultadoGrupo.Data.Add(new LineaBalanceGeneralAgrupado
            {
                Codigo = item.Key,
                Texto = item.First().Texto,
                ValorPrevio = previo,
                ValorActual = actual,
                ValorVariacion = actual - previo,
                ValorPorcentaje = previo == 0 ? 100 : ((actual / previo) - 1)
            });
        }
        return balanceGeneralResultadoGrupo;
    }

    public NotaBalanceGeneral GuardarNota(Int32 clienteId, NotaBalanceGeneralRequest notaBalanceGeneralRequest)
    {
        Int32 id = _balanceGeneralRepository.GuardarNota(clienteId, notaBalanceGeneralRequest);
        return new NotaBalanceGeneral()
        {
            Id = id,
            Account = notaBalanceGeneralRequest.Account,
            NoteHead = notaBalanceGeneralRequest.NoteHead,
            NoteValue = notaBalanceGeneralRequest.NoteValue,
            NoteContent = notaBalanceGeneralRequest.NoteContent,
            Year = notaBalanceGeneralRequest.Year,
            IdCompany = clienteId,
            Month = notaBalanceGeneralRequest.Month
        };
    }

    private LineaBalanceGeneral procesarUtilidadMes(List<string> etiquetas, List<LineaBalanceGeneral> data, string Language)
    {
        List<string> etiquetasBuscar = new List<string>() { "1", "2", "3", "5", "6", "7" };
        LineaBalanceGeneral resultado = new LineaBalanceGeneral() { Texto = "es".Equals(Language) ? "Utilidad mes" : "Monthly profit\r\n", Data = new List<LineaBalanceGeneral>() };
        decimal total = 0;
        foreach (var item in etiquetas)
        {
            List<LineaBalanceGeneral> objetoEncontrados = data.FindAll(x => etiquetasBuscar.Contains(x.Etiqueta));
            LineaBalanceGeneral consolidado = new LineaBalanceGeneral()
            {
                Mes = Convert.ToInt16(item),
                Total = obtenerIngresos(item, data)
            };
            if (objetoEncontrados.Count > 0)
            {
                foreach (var objetoEncontrado in objetoEncontrados)
                {
                    consolidado.Total -= objetoEncontrado.Data.Find(x => x.Mes.ToString() == item).Total;
                }
            }
            resultado.Data.Add(consolidado);
            total += consolidado.Total;
        }
        resultado.Data.Add(new LineaBalanceGeneral()
        {
            Total = total
        });
        return resultado;
    }

    /// <summary>
    /// Método que se encarga de obtener el valor 
    /// </summary>
    /// <param name="mes">El mes que se está consultando</param>
    /// <param name="data">Los valores a consultar</param>
    /// <returns>Valor con el valor de los ingresos</returns>
    private decimal obtenerIngresos(string mes, List<LineaBalanceGeneral> data)
    {
        LineaBalanceGeneral balanceGeneralEncabezado = data.Find(x => "4".Equals(x.Etiqueta));
        return balanceGeneralEncabezado == null ? 0 : balanceGeneralEncabezado.Data.Find(x => x.Mes.ToString() == mes).Total;
    }

    /// <summary>
    /// Métodos para calcular el total general
    /// </summary>
    /// <param name="etiquetas">Los valores de las etiquetas que se visualizan</param>
    /// <param name="data">Datos para hacer cálculos de utilidad de mes</param>
    /// <returns>Retorna un consolidado de las utilidades de mes</returns>
    private List<LineaBalanceGeneral> procesarTotalGeneral(List<string> etiquetas, List<LineaBalanceGeneral> data)
    {
        List<LineaBalanceGeneral> resultado = new List<LineaBalanceGeneral>();
        decimal total = 0;
        foreach (var item in etiquetas)
        {
            LineaBalanceGeneral consolidado = new LineaBalanceGeneral()
            {
                Mes = Convert.ToInt16(item),
                Total = 0
            };
            foreach (var valor in data)
            {
                List<LineaBalanceGeneral> objetoEncontrados = valor.Data.FindAll(x => x.Mes.ToString() == item);

                if (objetoEncontrados.Count > 0)
                {
                    foreach (var objetoEncontrado in objetoEncontrados)
                    {
                        consolidado.Total += objetoEncontrado.Total;
                    }
                }
            }
            resultado.Add(consolidado);
            total += consolidado.Total;
        }
        resultado.Add(new LineaBalanceGeneral()
        {
            Total = total
        });
        return resultado;
    }

    /// <summary>
    /// Método para procesar los datos de la consulta
    /// </summary>
    /// <param name="etiquetas">Las etiquetas que se van a mostrar</param>
    /// <param name="data">Los datos que se van a procesar</param>
    /// <param name="language">Lenguaje para la traducción</param>
    /// <param name="omitirValores">omitir valores</param>
    /// <returns>Una lista con los valores procesados</returns>
    private List<LineaBalanceGeneral> procesar(List<string> etiquetas, List<LineaBalanceGeneral> data, string language, bool omitirValores)
    {
        List<LineaBalanceGeneral> resultado = new List<LineaBalanceGeneral>();
        decimal total = 0;
        foreach (var item in etiquetas)
        {
            LineaBalanceGeneral objetoEncontrado = data.Find(x => (x.Mes.ToString() == item) || (_mesesService.ObtenerNombreMes(x.Mes, language).Equals(item)));
            if (objetoEncontrado != null)
            {
                resultado.Add(objetoEncontrado);
                total += objetoEncontrado.Total;
            }
            else
            {
                if (omitirValores)
                {
                    resultado.Add(new LineaBalanceGeneral()
                    {
                        Mes = Convert.ToInt32(item),
                        Total = 0
                    });
                }
                else
                {
                    Nullable<Int32> monthNumber = _mesesService.ObtenerNumeroMes(item, language);
                    if (monthNumber.HasValue)
                    {
                        resultado.Add(new LineaBalanceGeneral()
                        {
                            Mes = monthNumber.Value,
                            Total = 0
                        });
                    }
                }
            }
        }
        resultado.Add(new LineaBalanceGeneral()
        {
            Total = total
        });
        return resultado;
    }


    /// <summary>
    /// Método para procesar la utilidad acumulada
    /// </summary>
    /// <param name="utilidadMes">La utilidad del mes</param>
    /// <param name="Language">Lenguaje para la traducción</param>
    /// <returns>Linea balance general con la utilidad acumulada</returns>
    private LineaBalanceGeneral procesarUtilidadAcumulada(LineaBalanceGeneral utilidadMes, string Language)
    {
        LineaBalanceGeneral resultado = new LineaBalanceGeneral() { Texto = "es".Equals(Language) ? "Utilidad acumualada" : "Accumulated profit", Data = new List<LineaBalanceGeneral>() };
        bool procesado = false;
        decimal total = 0;
        decimal anterior = 0;
        foreach (var item in utilidadMes.Data.FindAll(x => x.Mes > 0))
        {
            if (procesado)
            {
                resultado.Data.Add(new LineaBalanceGeneral()
                {
                    Total = anterior + item.Total
                });
                anterior = item.Total;
                total += item.Total;
            }
            else
            {
                resultado.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Total
                });
                anterior = item.Total;
                total = item.Total;
                procesado = true;
            }
        }
        resultado.Data.Add(new LineaBalanceGeneral()
        {
            Total = total
        });
        return resultado;
    }

    /// <summary>
    /// Método para generar las traducciones en inglés
    /// </summary>
    /// <param name="Language"></param>
    /// <param name="etiquetasTraducir">El valor que se va a traducir</param>
    /// <returns>El valor traducido</returns>
    private List<String> traducccionEtiquetas(String Language, List<string> etiquetasTraducir, bool viewAccountCode)
    {
        List<string> etiquetas = new List<string>() { Language.Equals("es") ? "Etiqueta" : "Label" };
        if(viewAccountCode)
        {
            etiquetas.Add(Language.Equals("es") ? "Código Cuenta" : "Account Code");
        }
        foreach (var item in etiquetasTraducir)
        {
            etiquetas.Add(_mesesService.ObtenerNombreMes(Convert.ToInt16(item), Language));
        }
        etiquetas.Add(Language.Equals("es") ? "Total general" : "Grand total");
        return etiquetas;
    }

    
}
