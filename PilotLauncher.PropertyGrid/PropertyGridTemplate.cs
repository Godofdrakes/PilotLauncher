using System;
using System.Windows;
using System.Windows.Markup;

namespace PilotLauncher.PropertyGrid;

[ContentProperty(nameof(DataTemplate))]
public class PropertyGridTemplate
{
	public bool? IsReadOnly { get; set; }
	public bool? IsValueType { get; set; }
	public Type? PropertyType { get; set; }

	public DataTemplate? DataTemplate { get; set; }
}