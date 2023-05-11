using System;
using System.Windows;

namespace PilotLauncher.PropertyGrid;

public class PropertyGridTemplate
{
	public int Priority { get; set; }
	public Func<PropertyGridItem,bool>? Match { get; set; }
	public DataTemplate? DataTemplate { get; set; }
}