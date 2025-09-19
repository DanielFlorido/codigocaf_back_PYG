using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Aplicacion.Interfaces.Mail;
using CodigoCAFBack.Aplicacion.Interfaces.PYG;
using CodigoCAFBack.Aplicacion.Interfaces.Utils;
using CodigoCAFBack.Aplicacion.Services.Comun;
using CodigoCAFBack.Aplicacion.Services.Mail;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Aplicacion.Services.PYG;
public class PYGService : IPYGService
    {
    private readonly IPYGRepository _pygRepository;
    private readonly IMesesService _mesesService;
    private readonly ILogService _logService;
    private readonly IExcelService _excelService;
    private readonly IClienteLogoService _clienteLogoService;
    private readonly IUsuarioMulticlienteService _usuarioMulticlienteService;
    private readonly IMailService _mailService;
    public PYGService(IPYGRepository pYGRepository, IMesesService mesesService,
        IExcelService excelService,
        IClienteLogoService clienteLogoService,
        IUsuarioMulticlienteService usuarioMulticlienteService, IMailService mailService)
    {
        _pygRepository = pYGRepository;
        _mesesService = mesesService;
        _excelService = excelService;
        _clienteLogoService = clienteLogoService;
        _usuarioMulticlienteService = usuarioMulticlienteService;
        _mailService = mailService;
    }
    public byte[] GenerarExcel(Int32 idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        var logoClienteRequest = new LogoClienteRequest
        {
            IdCliente = idCliente,
            IdUsuario = balanceGeneralRequest.UserId,
        };
        var logocliente = _clienteLogoService.BuscarLogoCliente(logoClienteRequest);
        PYGResultado resultado = ConsultarPYGGrupos(idCliente, balanceGeneralRequest);
        var clientes = _usuarioMulticlienteService.ConsultarClientesPorUsuario(balanceGeneralRequest.UserId);
        string meses = balanceGeneralRequest.Year.HasValue ?
            balanceGeneralRequest.Month.HasValue ? _mesesService.ConsultarMes(balanceGeneralRequest.Language, balanceGeneralRequest.Month.Value) : "es".Equals(balanceGeneralRequest.Language) ? "Todos Los Meses" : "Every Month" : "";
        
        PYGResultado PYGResultado = new PYGResultado();
        
        if (balanceGeneralRequest.Grupo != null)
        {
            PYGResultado = ConsultarPYGGrupos(idCliente, balanceGeneralRequest);
        }
        else if (balanceGeneralRequest.CodigoPosPre != null)
        {
            PYGResultado = ConsultarPYGCuentas(idCliente, balanceGeneralRequest);
        }
        else
        {
            PYGResultado.Data = new List<LineaBalanceGeneral>();
        }
        return _excelService.GenerarExcel(idCliente, balanceGeneralRequest, logocliente, resultado, clientes, meses, PYGResultado);
    }
    public PYGResultado ConsultarPYGGrupos(int idCliente, BalanceGeneralRequest request)
    {
        List<LineaBalanceGeneral> grupos = _pygRepository.ConsultarPYGGrupos(idCliente, request);
        PYGResultado resultado = new PYGResultado();
        List<string> etiquetas = new List<string>();
        foreach(var item in grupos.GroupBy(x => x.Mes))
        {
            etiquetas.Add(item.Key.ToString());
        }
        resultado.Data = new List<LineaBalanceGeneral>();
        foreach (var item in grupos.GroupBy(x=> x.Etiqueta))
        {
            resultado.Data.Add(new LineaBalanceGeneral()
                {
                    Etiqueta = item.Key,
                    Texto = item.ToList().First().Texto,
                    Data = procesar(
                    etiquetas,
                    item.ToList(),
                    request.Language,
                    true,
                    !(request.Month.HasValue && request.Month.Value >= 1) 
                )
            });
        }
        if (request.Month.HasValue && request.Month.Value >= 1)
        {
            foreach (var item in resultado.Data)
            {
                item.Data.RemoveAll(removed => removed.Mes != request.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
        }
        resultado.Etiquetas = traducccionEtiquetas(request.Language, etiquetas, request.ViewAccountCode, !(request.Month.HasValue && request.Month.Value >= 1));
        resultado.UtilidadBruta = ProcesarUtilidadBruta(request, grupos);
        resultado.UtilidadBruta.Data.Add(new LineaBalanceGeneral()
        {
            Total = resultado.UtilidadBruta.Data.Sum(x => x.Total)
        });
        resultado.UtilidadOperacional = ProcesarUtilidadOperacional(request, grupos, resultado.UtilidadBruta);
        resultado.UtilidadOperacional.Data.Add(new LineaBalanceGeneral()
        {
            Total = resultado.UtilidadOperacional.Data.Sum(x => x.Total)
        });
        resultado.UtilidadAntesImpuestos = ProcesarUtilidadNetaAntesDeImpuesto(request, grupos, resultado.UtilidadOperacional);
        resultado.UtilidadAntesImpuestos.Data.Add(new LineaBalanceGeneral()
        {
            Total = resultado.UtilidadAntesImpuestos.Data.Sum(x => x.Total)
        });
        resultado.UtilidadNeta = ProcesarUtilidadNeta(request, grupos, resultado.UtilidadAntesImpuestos);
        resultado.UtilidadNeta.Data.Add(new LineaBalanceGeneral()
        {
            Total = resultado.UtilidadNeta.Data.Sum(x => x.Total)
        });


        return resultado;
    }
    public PYGResultado ConsultarPYGCuentas(int idCliente, BalanceGeneralRequest request)
    {
        List<LineaBalanceGeneral> cuentas = _pygRepository.ConsultarPYGCuentas(idCliente, request);

        PYGResultado result = new PYGResultado();
        result.Data = new List<LineaBalanceGeneral>();
        foreach (var item in cuentas.GroupBy(x => x.Etiqueta))
        {
            result.Data.Add(new LineaBalanceGeneral()
            {
                Etiqueta = item.Key,
                Texto = item.ToList().FirstOrDefault().Texto,
                Data = procesar(request.etiquetas, item.ToList(), request.Language, false)
            });
        }
        if (request.Month.HasValue && request.Month.Value >= 1)
        {
            foreach (var item in result.Data)
            {
                item.Data.RemoveAll(removed => removed.Mes != request.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
        }
        result.Etiquetas = request.etiquetas;
        return result;
    }
    public LineaBalanceGeneral ProcesarUtilidadBruta(BalanceGeneralRequest request, List<LineaBalanceGeneral> grupos)
    {
        return new LineaBalanceGeneral()
        {
            Etiqueta = "UB",
            Texto = request.Language == "es" ? "Utilidad Bruta" : "Gross Profit",
            Data = grupos
                .GroupBy(x => x.Mes)
                .Select(g => new LineaBalanceGeneral()
                {
                    Mes = g.Key,
                    Year = g.First().Year,
                    Total =
                        g.Where(x => x.Etiqueta == "41").Sum(x => x.Total) -
                        g.Where(x => x.Etiqueta == "61").Sum(x => x.Total) -
                        g.Where(x => x.Etiqueta == "62").Sum(x => x.Total)
                })
                .ToList()
        };
    }

    public LineaBalanceGeneral ProcesarUtilidadOperacional(BalanceGeneralRequest request, List<LineaBalanceGeneral> grupos, LineaBalanceGeneral utilidadBruta)
    {
        return new LineaBalanceGeneral()
        {
            Etiqueta = "UP",
            Texto = request.Language == "es" ? "Utilidad Operacional" : "Operational Profit",
            Data = utilidadBruta.Data
                .Select(ub => new LineaBalanceGeneral()
                {
                    Mes = ub.Mes,
                    Year = ub.Year,
                    Total = ub.Total -
                            grupos.Where(x => x.Mes == ub.Mes && x.Etiqueta == "51").Sum(x => x.Total) -
                            grupos.Where(x => x.Mes == ub.Mes && x.Etiqueta == "52").Sum(x => x.Total)
                })
                .ToList()
        };
    }

    public LineaBalanceGeneral ProcesarUtilidadNetaAntesDeImpuesto(BalanceGeneralRequest request, List<LineaBalanceGeneral> grupos, LineaBalanceGeneral utilidadOperacional)
    {
        return new LineaBalanceGeneral()
        {
            Etiqueta = "UNI",
            Texto = request.Language == "es" ? "Utilidad Neta antes de Impuestos" : "Profit before Taxes",
            Data = utilidadOperacional.Data
                .Select(up => new LineaBalanceGeneral()
                {
                    Mes = up.Mes,
                    Year = up.Year,
                    Total = up.Total -
                            grupos.Where(x => x.Mes == up.Mes && x.Etiqueta == "53").Sum(x => x.Total) +
                            grupos.Where(x => x.Mes == up.Mes && x.Etiqueta == "42").Sum(x => x.Total)
                })
                .ToList()
        };
    }

    public LineaBalanceGeneral ProcesarUtilidadNeta(BalanceGeneralRequest request, List<LineaBalanceGeneral> grupos, LineaBalanceGeneral utilidadAntesImpuestos)
    {
        return new LineaBalanceGeneral()
        {
            Etiqueta = "UN",
            Texto = request.Language == "es" ? "Utilidad Neta" : "Net Profit",
            Data = utilidadAntesImpuestos.Data
                .Select(uni => new LineaBalanceGeneral()
                {
                    Mes = uni.Mes,
                    Year = uni.Year,
                    Total = uni.Total -
                            grupos.Where(x => x.Mes == uni.Mes && x.Etiqueta == "54").Sum(x => x.Total)
                })
                .ToList()
        };
    }

    private List<LineaBalanceGeneral> procesar(
        List<string> etiquetas,
        List<LineaBalanceGeneral> data,
        string language,
        bool omitirValores,
        bool incluirTotalGeneral = true 
    )
    {
        List<LineaBalanceGeneral> resultado = new List<LineaBalanceGeneral>();
        decimal total = 0;

        foreach (var item in etiquetas)
        {
            LineaBalanceGeneral objetoEncontrado = data.Find(x =>
                (x.Mes.ToString() == item) ||
                (_mesesService.ObtenerNombreMes(x.Mes, language).Equals(item))
            );

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

        if (incluirTotalGeneral)
        {
            resultado.Add(new LineaBalanceGeneral()
            {
                Total = total
            });
        }

        return resultado;
    }
    private List<String> traducccionEtiquetas(String Language, List<string> etiquetasTraducir, bool viewAccountCode, Boolean general)
    {
        List<string> etiquetas = new List<string>() { Language.Equals("es") ? "Etiqueta" : "Label" };
        if (viewAccountCode)
        {
            etiquetas.Add(Language.Equals("es") ? "Código Cuenta" : "Account Code");
        }
        foreach (var item in etiquetasTraducir)
        {
            etiquetas.Add(_mesesService.ObtenerNombreMes(Convert.ToInt16(item), Language));
        }
        if (general)
        {
            etiquetas.Add(Language.Equals("es") ? "Total general" : "Grand total");
        }
        
        return etiquetas;
    }

    public PYGResultado ConsultarPYGSubCuentas(int idCliente, BalanceGeneralRequest request)
    {
        List<LineaBalanceGeneral> subcuentas = _pygRepository.ConsultarPYGSubcuentas(idCliente, request);

        PYGResultado result = new PYGResultado();
        result.Data = new List<LineaBalanceGeneral>();
        foreach (var item in subcuentas.GroupBy(x => x.Etiqueta))
        {
            result.Data.Add(new LineaBalanceGeneral()
            {
                Etiqueta = item.Key,
                Texto = item.ToList().FirstOrDefault().Texto,
                Data = procesar(request.etiquetas, item.ToList(), request.Language, false)
            });
        }
        if (request.Month.HasValue && request.Month.Value >= 1)
        {
            foreach (var item in result.Data)
            {
                item.Data.RemoveAll(removed => removed.Mes != request.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
        }
        result.Etiquetas = request.etiquetas;
        return result;
    }

    public PYGResultado ConsultarPYGAuxiliares(int idCliente, BalanceGeneralRequest request)
    {
        List<LineaBalanceGeneral> auxiliares = _pygRepository.ConsultarPYGAuxiliares(idCliente, request);

        PYGResultado result = new PYGResultado();
        result.Data = new List<LineaBalanceGeneral>();
        foreach (var item in auxiliares.GroupBy(x => x.Etiqueta))
        {
            result.Data.Add(new LineaBalanceGeneral()
            {
                Etiqueta = item.Key,
                Texto = item.ToList().FirstOrDefault().Texto,
                Data = procesar(request.etiquetas, item.ToList(), request.Language, false)
            });
        }
        if (request.Month.HasValue && request.Month.Value >= 1)
        {
            foreach (var item in result.Data)
            {
                item.Data.RemoveAll(removed => removed.Mes != request.Month.Value);
                item.Data.Add(new LineaBalanceGeneral()
                {
                    Total = item.Data.Sum(x => x.Total)
                });
            }
        }
        result.Etiquetas = request.etiquetas;
        return result;
    }
    public void EnviarExcel(Int32 idCliente, BalanceGeneralRequest request)
    {
        _mailService.SendEmailAsync(_usuarioMulticlienteService.ObtenerCorreoParaCliente(idCliente), TituloCorreo(request.Language), CuerpoCorreo(request.Language), new List<byte[]>() { this.GenerarExcel(idCliente, request) }
            , new List<string>() { "PYG.xlsx" }, false);
    }

    public string TituloCorreo(string Lenguaje)
    {
        return "es".Equals(Lenguaje) ? "Estado de resultados PYG" : "Income Statement";

    }

    public string CuerpoCorreo(string Lenguaje)
    {
        return "es".Equals(Lenguaje) ? "Hola, adjunto encontraras el estado de resultados PYG" : "Hello, attached you will find the Income Statement.";
    }

}

