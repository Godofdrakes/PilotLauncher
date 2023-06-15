using System;
using System.Reflection;
using PilotLauncher.Common;

namespace PilotLauncher.PropertyGrid;

[AttributeUsage(AttributeTargets.Property)]
public class PropertyGridCategoryAttribute : Attribute
{
	public static string GetValue(PropertyInfo propertyInfo)
	{
		var attr = propertyInfo.GetCustomAttribute<PropertyGridCategoryAttribute>();
		if (attr is not null)
		{
			return attr.Value;
		}

		if (propertyInfo.DeclaringType is not null)
		{
			return propertyInfo.DeclaringType.Name;
		}

		return string.Empty;
	}

	public PropertyGridCategoryAttribute(string value)
	{
		Ensure.False(() => string.IsNullOrEmpty(value));

		Value = value;
	}

	public string Value { get; }
}