using System;
using System.Windows;
using System.Windows.Markup;

namespace PilotLauncher.PropertyGrid;

[ContentProperty(nameof(DataTemplate))]
public class PropertyGridTemplate : PropertyInfoMatcher
{
	public DataTemplate? DataTemplate { get; set; }
}