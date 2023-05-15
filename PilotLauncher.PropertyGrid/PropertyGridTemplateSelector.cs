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

	public PropertyGridTemplateSelector? BasedOn { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not PropertyGridItem propertyGridItem)
			return null;

		return GetAllTemplates()
			.Where(template => template.Match?.Invoke(propertyGridItem) ?? true)
			.OrderByDescending(template => template.Priority)
			.Select(template => template.DataTemplate)
			.FirstOrDefault();
	}

	private IEnumerable<PropertyGridTemplate> GetAllTemplates() => this
		.Flatten(selector => selector.BasedOn)
		.SelectMany(selector => selector.Templates);
}