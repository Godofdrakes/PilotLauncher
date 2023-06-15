using System.Windows;
using System.Windows.Controls;

namespace PilotLauncher.PropertyGrid;

public class PropertyGridTemplateSelector : DataTemplateSelector
{
	public DataTemplate? UnknownItemTemplate { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not PropertyGridItem propertyGridItem)
			return UnknownItemTemplate;

		if (container is not FrameworkElement element)
			return UnknownItemTemplate;
		
		return element.TryFindResource(propertyGridItem.Template) as DataTemplate
			?? UnknownItemTemplate;
	}
}