using System;

namespace PilotLauncher.PropertyGrid;

public class PropertyInfoMatcher
{
	public bool? IsReadOnly { get; set; }

	public Type? ExactType { get; set; }
	public Type? BaseType { get; set; }
	public bool? IsValueType { get; set; }
	public bool? IsEnum { get; set; }

	protected virtual bool MatchType(Type propertyType)
	{
		if (ExactType is not null && propertyType != ExactType)
			return false;

		if (BaseType is not null && !propertyType.IsAssignableTo(BaseType))
			return false;
		
		if (IsValueType is not null && propertyType.IsValueType != IsValueType)
			return false;

		if (IsEnum is not null && propertyType.IsEnum != IsEnum)
			return false;

		return true;
	}

	public bool Match(PropertyGridItem propertyGridItem)
	{
		if (IsReadOnly is not null && propertyGridItem.IsReadOnly != IsReadOnly)
			return false;

		return MatchType(propertyGridItem.PropertyInfo.PropertyType);
	}
}