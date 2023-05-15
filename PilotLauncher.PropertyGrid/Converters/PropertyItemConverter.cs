using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace PilotLauncher.PropertyGrid;

[ValueConversion(typeof(object), typeof(IEnumerable<PropertyGridItem>))]
public class PropertyItemConverter : IValueConverter
{
	public Func<PropertyInfo,bool>? Filter { get; set; }

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is null)
		{
			return Enumerable.Empty<PropertyGridItem>();
		}

		return PropertyGridItem.Scan(value, Filter);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}