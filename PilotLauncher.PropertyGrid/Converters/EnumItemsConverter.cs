using System;
using System.Globalization;
using System.Windows.Data;

namespace PilotLauncher.PropertyGrid;

[ValueConversion(typeof(Type), typeof(Array))]
public class EnumItemsConverter : IValueConverter
{
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not Type type)
			return null;

		if (type is not { IsEnum: true })
			return null;

		return Enum.GetValues(type);
	}

	public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}