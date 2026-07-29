using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Gamelistify.Helpers;

public class BoolToStarBrushConverter : IValueConverter
{
    private static readonly IBrush FilledBrush = new SolidColorBrush(Color.Parse("#E07C30"));

    private static readonly IBrush EmptyBrush = new SolidColorBrush(Color.Parse("#8B7E70"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool filled && filled)
            return FilledBrush;
        return EmptyBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
