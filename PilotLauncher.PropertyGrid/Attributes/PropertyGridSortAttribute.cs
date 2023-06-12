using System;
using System.Reflection;

namespace PilotLauncher.PropertyGrid;

[AttributeUsage(AttributeTargets.Property)]
public class PropertyGridSortAttribute : Attribute
{
	public static int GetValue(PropertyInfo propertyInfo)
	{
		return propertyInfo.GetCustomAttribute<PropertyGridSortAttribute>()?.Value ?? 0;
	}

	public PropertyGridSortAttribute(int value)
	{
		Value = value;
	}

	public int Value { get; }
}