using FilaVirtual.App.Models;
using System.Globalization;

namespace FilaVirtual.App.Converters
{
    /// <summary>
    /// Convierte EstadoPedido a bool (true si está EnCola)
    /// </summary>
    public class EstadoEnColaConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is EstadoPedido estado && estado == EstadoPedido.EnCola;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


