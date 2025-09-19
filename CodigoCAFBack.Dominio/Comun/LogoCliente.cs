using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Dominio.Comun;

public class LogoCliente
{
    public Int32 Id { get; set; }

    public Int32 IdCliente { get; set; }

    public Int32 IdUsuario { get; set; }

    public byte[]? LogoPrincipal { get; set; }

    public byte[]? LogoAuxiliar { get; set; }

    public byte[]? FirmaContador { get; set; }

    public byte[]? FirmaRevisor { get; set; }

    public byte[]? FirmaGerente { get; set; }

}
