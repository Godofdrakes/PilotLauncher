using System;
using System.Globalization;
using System.Windows.Data;

namespace PilotLauncher.WPF;

[ValueConversion(typeof(IConvertible), typeof(double))]
public class NumberConverter : IValueConverter
{
	public static NumberConverter Instance { get; } = new();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return System.Convert.ToDouble(value, culture);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return System.Convert.ChangeType(value, targetType, culture);
	}
}