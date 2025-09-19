using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodigoCAFBack.Dominio.Comun;

[Table("UsuarioMulticliente")]
public class UsuarioMulticliente
{

    [Key]
    public Int32 Id { get; set; }

    public Int32 IdUsuario { get; set; }

    public Int32 IdCliente { get; set; }

    public bool Estado { get; set; }

}
