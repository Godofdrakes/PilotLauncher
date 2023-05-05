using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PilotLauncher.PropertyGrid;

public class PropertyGridItem : ReactiveObject
{
	[Reactive] public string PropertyName { get; set; }
	[Reactive] public Type PropertyType { get; set; }
	[Reactive] public object? Value { get; set; }
	[Reactive] public bool IsNullable { get; set; }
	[Reactive] public bool IsReadOnly { get; set; }
	[Reactive] public IValueConverter? Converter { get; set; }

	public string TextValue => _textValue.Value;
	private readonly ObservableAsPropertyHelper<string> _textValue;

	private PropertyGridItem()
	{
		_textValue = this.WhenAnyValue(instance => instance.Value)
			.Select(value => value?.ToString() ?? string.Empty)
			.ToProperty(this, instance => instance.TextValue);
	}

	private static readonly NullabilityInfoContext NullabilityContext = new();

	public static PropertyGridItem Create(PropertyInfo propertyInfo)
	{
		var nullabilityInfo = NullabilityContext.Create(propertyInfo);

		return new PropertyGridItem()
		{
			PropertyName = propertyInfo.Name,
			PropertyType = propertyInfo.PropertyType,
			IsReadOnly = !propertyInfo.CanWrite,
			IsNullable = nullabilityInfo.WriteState is not NullabilityState.NotNull,
		};
	}

	public static IEnumerable<PropertyInfo> Scan(object propertySource) => propertySource.GetType()
		.GetProperties(BindingFlags.Public | BindingFlags.Instance)
		.Where(info => info.CanRead);
}