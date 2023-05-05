using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PilotLauncher.PropertyGrid;

[ContentProperty(nameof(Templates))]
public class PropertyGridTemplateSelector : DataTemplateSelector
{
	public ObservableCollection<PropertyGridTemplate> Templates { get; } = new();

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not PropertyGridItem propertyGridItem)
			return null;

		return Filter(Templates, propertyGridItem)
			.Select(template => template.DataTemplate)
			.FirstOrDefault();
	}

	private IEnumerable<PropertyGridTemplate> Filter(
		IEnumerable<PropertyGridTemplate> templates,
		PropertyGridItem propertyGridItem)
	{
		return templates
			.Where(template => template.IsReadOnly is not null || template.IsReadOnly == propertyGridItem.IsReadOnly)
			.Where(template => template.PropertyType is not null || template.PropertyType == propertyGridItem.PropertyType);
	}
}