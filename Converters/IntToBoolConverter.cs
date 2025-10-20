using System.Globalization;

namespace FilaVirtual.App.Converters
{
    /// <summary>
    /// Convierte un entero a bool (true si > 0)
    /// </summary>
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is int intValue && intValue > 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
