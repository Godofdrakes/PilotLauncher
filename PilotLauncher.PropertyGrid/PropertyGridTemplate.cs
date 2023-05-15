using System;
using System.Windows;
using System.Windows.Markup;

namespace PilotLauncher.PropertyGrid;

[ContentProperty(nameof(DataTemplate))]
public class PropertyGridTemplate
{
	public int Priority { get; set; }
	public Func<PropertyGridItem, bool>? Match { get; set; }
	public DataTemplate? DataTemplate { get; set; }
}