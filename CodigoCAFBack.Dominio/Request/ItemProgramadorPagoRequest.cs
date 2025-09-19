using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodigoCAFBack.Dominio.Request;

public class ItemProgramadorPagoRequest
{

    public Nullable<Int32> Id { get; set; }

    public string ItemEs { get; set; }

    public string ItemEn { get; set; }

    public string Cuentas { get; set; }

    public int IdCliente { get; set; }


}
