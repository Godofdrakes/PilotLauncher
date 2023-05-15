using System;
using System.Globalization;
using System.Windows.Data;

namespace PilotLauncher.PropertyGrid;

public class NumberConverter : IValueConverter
{
	public TypeCode InputType { get; set; } = TypeCode.Int32;
	public TypeCode OutputType { get; set; } = TypeCode.Single;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return System.Convert.ChangeType(value, OutputType);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return System.Convert.ChangeType(value, InputType);
	}
}