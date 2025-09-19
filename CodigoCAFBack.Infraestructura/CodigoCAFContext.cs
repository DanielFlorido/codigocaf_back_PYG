using CodigoCAFBack.Dominio.Comun;
using CodigoCAFBack.Dominio.Entidades;
using CodigoCAFBack.Dominio.Log;
using Microsoft.EntityFrameworkCore;

namespace CodigoCAFBack.Infraestructura;

public partial class CodigoCAFContext : DbContext
{
    public CodigoCAFContext(DbContextOptions<CodigoCAFContext> options) : base(options)
    {
    }

    public CodigoCAFContext()
    {
    }

    public virtual DbSet<UsuarioMulticliente> UsuarioMulticlientes { get; set; }

    public virtual DbSet<ParametroBalanceGeneral> ParametroBalanceGenerals { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
