using CodigoCAFBack.Dominio.Comun;

namespace CodigoCAFBack.Aplicacion.Interfaces.Comun;

public interface IMesesService
{

    List<Mes> ConsultarMeses(string language);

    string ConsultarMes(String language, Int32 idMes);

    string ObtenerNombreMes(int MonthNumber, string Language);

    Nullable<Int32> ObtenerNumeroMes(string MonthName, string Language);

}
