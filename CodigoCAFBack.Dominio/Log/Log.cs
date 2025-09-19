using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodigoCAFBack.Dominio.Log;

[Table("Log")]
public class Log
{

    [Key]
    public Int32 Id { get; set; }

    public string Controlador { get; set; }

    public string Metodo { get; set; }

    public string Mensaje { get; set; }

    public DateTime Fecha { get; set; }

}
