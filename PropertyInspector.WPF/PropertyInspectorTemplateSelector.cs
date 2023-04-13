using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PropertyInspector.Interfaces;

namespace PropertyInspector.WPF;

public class PropertyInspectorTemplateSelector : DataTemplateSelector
{
	public Dictionary<Type, DataTemplate> PropertyTemplates { get; } = new();

	public DataTemplate? DefaultTemplate { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not IPropertyInspector inspector)
			return null;

		if (PropertyTemplates.TryGetValue(inspector.PropertyType, out var dataTemplate))
		{
			return dataTemplate;
		}

		return DefaultTemplate;
	}
}