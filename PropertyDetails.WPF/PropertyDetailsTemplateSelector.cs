using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PropertyDetails.Interfaces;

namespace PropertyDetails.WPF;

public class PropertyDetailsTemplateSelector : DataTemplateSelector
{
	public Dictionary<Type, DataTemplate> PropertyTemplates { get; } = new();

	public DataTemplate? DefaultTemplate { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not IPropertyDetails inspector)
			return null;

		if (PropertyTemplates.TryGetValue(inspector.PropertyType, out var dataTemplate))
		{
			return dataTemplate;
		}

		return DefaultTemplate;
	}
}