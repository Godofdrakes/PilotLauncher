using System;
using System.Globalization;
using System.Windows.Data;

namespace PilotLauncher.WPF;

[ValueConversion(typeof(IConvertible), typeof(double))]
public class NumberConverter : IValueConverter
{
	public static NumberConverter BindToInteger { get; } = new(TypeCode.Int32);

	public TypeCode BindingType { get; }

	public NumberConverter(TypeCode bindingType)
	{
		this.BindingType = bindingType;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var type = Nullable.GetUnderlyingType(targetType) ?? targetType;
		return System.Convert.ChangeType(value, type, culture);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return System.Convert.ChangeType(value, BindingType, culture);
	}
}