using System;
using System.Globalization;
using System.Windows.Data;

namespace PilotLauncher.PropertyGrid;

[ValueConversion(typeof(int), typeof(int?))]
public class IndexConverter : IValueConverter
{
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		int? index = null;

		if (value is int i)
		{
			index = i;
		}

		return index;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var index = -1;

		if (value is int i)
		{
			index = i;
		}

		return index;
	}
}