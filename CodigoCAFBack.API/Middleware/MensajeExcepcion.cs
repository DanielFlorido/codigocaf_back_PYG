using System.Text.Json;

namespace CodigoCAFBack.API.Middleware;

public class MensajeExcepcion
{

    public string Mensaje { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

}
