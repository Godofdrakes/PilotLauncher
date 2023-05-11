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

		return Templates
			.Where(template => template.Match?.Invoke(propertyGridItem) ?? true)
			.Select(template => template.DataTemplate)
			.FirstOrDefault();
	}
}