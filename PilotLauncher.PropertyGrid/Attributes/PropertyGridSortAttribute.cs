using System;
using System.Reflection;
using JetBrains.Annotations;

namespace PilotLauncher.PropertyGrid;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
[MeansImplicitUse(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
[PublicAPI]
public class PropertyGridSortAttribute : Attribute
{
	public static int GetValue(PropertyInfo propertyInfo)
	{
		var attr = propertyInfo.GetCustomAttribute<PropertyGridSortAttribute>();
		if (attr is not null)
		{
			return attr.Value;
		}

		if (propertyInfo.DeclaringType is not null)
		{
			attr = propertyInfo.DeclaringType.GetCustomAttribute<PropertyGridSortAttribute>();
			if (attr is not null)
			{
				return attr.Value;
			}
		}

		return 0;
	}

	public PropertyGridSortAttribute(int value)
	{
		Value = value;
	}

	public int Value { get; }
}