using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Dominio.Comun;

namespace CodigoCAFBack.Aplicacion.Services.Comun;

public class MesesService : IMesesService
{

    public MesesService()
    {
    }

    public List<Mes> ConsultarMeses(string language)
    {
        List<Mes> meses = new List<Mes>();

        meses.Add(new Mes(){ Id = 0, Name = "es".Equals(language) ? "Todos los meses" : "All month" });

        for (int i = 1; i <= 12; i++)
        {
            meses.Add(new Mes() { Id = i, Name = ConsultarMes(language, i) });
        }

        return meses;
    }

    public string ConsultarMes(String language, Int32 idMes)
    {
        return ObtenerNombreMes(idMes, language);
    }


    /// <summary>
    /// Método para traducir el nombre de los meses 
    /// </summary>
    /// <param name="MonthNumber">El mes que se va a traducir</param>
    /// <param name="Language">El lenguaje en el cual se traduce el mes</param>
    /// <returns>El nombre del mes en el lenguaje que corresponde</returns>
    public string ObtenerNombreMes(int MonthNumber, string Language)
    {
        switch (MonthNumber)
        {
            case 1:
                return "es".Equals(Language) ? "Enero" : "January";
            case 2:
                return "es".Equals(Language) ? "Febrero" : "February";
            case 3:
                return "es".Equals(Language) ? "Marzo" : "March";
            case 4:
                return "es".Equals(Language) ? "Abril" : "April";
            case 5:
                return "es".Equals(Language) ? "Mayo" : "May";
            case 6:
                return "es".Equals(Language) ? "Junio" : "June";
            case 7:
                return "es".Equals(Language) ? "Julio" : "July";
            case 8:
                return "es".Equals(Language) ? "Agosto" : "August";
            case 9:
                return "es".Equals(Language) ? "Septiembre" : "September";
            case 10:
                return "es".Equals(Language) ? "Octubre" : "October";
            case 11:
                return "es".Equals(Language) ? "Noviembre" : "November";
            case 12:
                return "es".Equals(Language) ? "Diciembre" : "December";
            default:
                return "";
        }
    }

    /// <summary>
    /// Método para obtener el número mes de consulta
    /// </summary>
    /// <param name="MonthName">El nonbre del mes que se va a traducir</param>
    /// <param name="Language">El lenguaje en el cual se traduce el mes</param>
    /// <returns>El nombre del mes en el lenguaje que corresponde</returns>
    public Nullable<Int32> ObtenerNumeroMes(string MonthName, string Language)
    {
        if (MonthName.Equals("Enero") || MonthName.Equals("January"))
        {
            return 1;
        }
        else if (MonthName.Equals("Febrero") || MonthName.Equals("February"))
        {
            return 2;
        }
        else if (MonthName.Equals("Marzo") || MonthName.Equals("March"))
        {
            return 3;
        }
        else if (MonthName.Equals("Abril") || MonthName.Equals("April"))
        {
            return 4;
        }
        else if (MonthName.Equals("Mayo") || MonthName.Equals("May"))
        {
            return 5;
        }
        else if (MonthName.Equals("Junio") || MonthName.Equals("June"))
        {
            return 6;
        }
        else if (MonthName.Equals("Julio") || MonthName.Equals("July"))
        {
            return 7;
        }
        else if (MonthName.Equals("Agosto") || MonthName.Equals("August"))
        {
            return 8;
        }
        else if (MonthName.Equals("Septiembre") || MonthName.Equals("September"))
        {
            return 9;
        }
        else if (MonthName.Equals("Octubre") || MonthName.Equals("October"))
        {
            return 10;
        }
        else if (MonthName.Equals("Noviembre") || MonthName.Equals("November"))
        {
            return 11;
        }
        else if (MonthName.Equals("Diciembre") || MonthName.Equals("December"))
        {
            return 12;
        }
        else 
        {
            return null;
        }
    }

}
