using System.Globalization;

namespace FilaVirtual.App.Converters
{
    /// <summary>
    /// Convertidor que transforma bool a texto para el botón del micrófono
    /// </summary>
    public class BoolToMicTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool estaEscuchando)
            {
                return estaEscuchando ? "Detener" : "Activar";
            }
            return "Activar";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

