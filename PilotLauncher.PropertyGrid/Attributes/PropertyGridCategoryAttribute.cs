using System;
using System.Reflection;
using JetBrains.Annotations;
using PilotLauncher.Common;

namespace PilotLauncher.PropertyGrid;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
[MeansImplicitUse(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
[PublicAPI]
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
			attr = propertyInfo.DeclaringType.GetCustomAttribute<PropertyGridCategoryAttribute>();
			if (attr is not null)
			{
				return attr.Value;
			}
		}

		return string.Empty;
	}

	public PropertyGridCategoryAttribute(string value)
	{
		Ensure.That(() => !string.IsNullOrEmpty(value));

		Value = value;
	}

	public string Value { get; }
}