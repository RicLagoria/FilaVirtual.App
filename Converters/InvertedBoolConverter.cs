using System.Globalization;

namespace FilaVirtual.App.Converters
{
    /// <summary>
    /// Invierte un valor booleano
    /// </summary>
    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue;
        }
    }
}

