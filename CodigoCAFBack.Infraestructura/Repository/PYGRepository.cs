using CodigoCAFBack.Aplicacion.Interfaces.PYG;
using CodigoCAFBack.Dominio.Contratos;
using CodigoCAFBack.Dominio.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Infraestructura.Repository;

public class PYGRepository : IPYGRepository
{
    private readonly CodigoCAFContext _context;
    public PYGRepository(CodigoCAFContext context)
    {
        _context = context;
    }
    public List<LineaBalanceGeneral> ConsultarPYGGrupos(int idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneral>(
            "EXEC ConsultarPYGGrupos @IdCliente = {0}, @Month = {1}, @Year = {2}, @Language = {3}, @DateFrom = {4}, @DateTo = {5}",
            idCliente,
            balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : (object)DBNull.Value,
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : (object)DBNull.Value,
            balanceGeneralRequest.Language ?? (object)DBNull.Value,
            balanceGeneralRequest.DateFrom ?? (object)DBNull.Value,
            balanceGeneralRequest.DateTo ?? (object)DBNull.Value
        ).ToList();
    }   
    public List<LineaBalanceGeneral> ConsultarPYGCuentas(int idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneral>(
            "EXEC ConsultarPYGCuentas @IdCliente = {0}, @Month = {1}, @Year = {2}, @Language = {3}, @DateFrom = {4}, @DateTo = {5}, @Grupo={6}",
            idCliente,
            balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : (object)DBNull.Value,
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : (object)DBNull.Value,
            balanceGeneralRequest.Language ?? (object)DBNull.Value,
            balanceGeneralRequest.DateFrom ?? (object)DBNull.Value,
            balanceGeneralRequest.DateTo ?? (object)DBNull.Value,
            balanceGeneralRequest.Grupo ?? (object)DBNull.Value
        ).ToList();
    }

    public List<LineaBalanceGeneral> ConsultarPYGSubcuentas(int idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        return _context.Database.SqlQueryRaw<LineaBalanceGeneral>(
            "EXEC ConsultarPYGSubCuentas @IdCliente = {0}, @Month = {1}, @Year = {2}, @Language = {3}, @DateFrom = {4}, @DateTo = {5}, @CodigoPosPre = {6}",
            idCliente,
            balanceGeneralRequest.Month.HasValue ? balanceGeneralRequest.Month.Value : (object)DBNull.Value,
            balanceGeneralRequest.Year.HasValue ? balanceGeneralRequest.Year.Value : (object)DBNull.Value,
            balanceGeneralRequest.Language ?? (object)DBNull.Value,
            balanceGeneralRequest.DateFrom ?? (object)DBNull.Value,
            balanceGeneralRequest.DateTo ?? (object)DBNull.Value,
            balanceGeneralRequest.CodigoPosPre ?? (object) DBNull.Value
        ).ToList();
    }

    public List<LineaBalanceGeneral> ConsultarPYGAuxiliares(int idCliente, BalanceGeneralRequest balanceGeneralRequest)
    {
        throw new NotImplementedException();
    }
}
