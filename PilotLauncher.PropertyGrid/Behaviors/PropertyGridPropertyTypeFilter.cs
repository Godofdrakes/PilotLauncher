using System;

namespace PilotLauncher.PropertyGrid;

// Exclude properties of the specified type
public class PropertyGridPropertyTypeFilter : PropertyGridTypeFilter
{
	protected override void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		if (TargetType is null)
			throw new InvalidOperationException("PropertyGridPropertyTypeFilter: Invalid TargetType");

		var propertyType = e.PropertyInfo.PropertyType;

		if (propertyType == TargetType
			|| (FilterAssignableTo && propertyType.IsAssignableTo(TargetType))
			|| (FilterAssignableFrom && propertyType.IsAssignableFrom(TargetType)))
		{
			e.Cancel = true;
		}
	}
}