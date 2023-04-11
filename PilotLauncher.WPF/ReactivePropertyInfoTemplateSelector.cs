using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using PilotLauncher.Plugins;

namespace PilotLauncher.WPF;

public class DataTemplateTypeMap : Dictionary<Type,DataTemplate> {}

[ContentProperty(nameof(DefaultTemplate))]
public class ReactivePropertyInfoTemplateSelector : DataTemplateSelector
{
	public DataTemplateTypeMap Templates { get; set; } = new();

	public DataTemplate? DefaultTemplate { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not ReactivePropertyInfo info)
			return null;

		if (Templates.TryGetValue(info.PropertyInfo.PropertyType, out var template))
		{
			return template;
		}

		return DefaultTemplate;
	}
}