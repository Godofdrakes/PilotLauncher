using System;
using System.Reflection;
using System.Text;
using PilotLauncher.Common;

namespace PilotLauncher.PropertyGrid;

[AttributeUsage(AttributeTargets.Property)]
public class PropertyGridTemplateAttribute : Attribute
{
	public string Template { get; }

	public PropertyGridTemplateAttribute(string template)
	{
		Ensure.False(() => string.IsNullOrEmpty(template));

		Template = template;
	}

	public static string GetValue(PropertyInfo propertyInfo, bool isReadOnly)
	{
		var attribute = propertyInfo.GetCustomAttribute<PropertyGridTemplateAttribute>();
		if (attribute is not null)
		{
			return attribute.Template;
		}

		var builder = new StringBuilder("PilotLauncher.PropertyGridItem");

		var propertyType = propertyInfo.PropertyType;

		if (propertyType.IsEnum)
		{
			builder.Append(".Enum");
		}
		else
		{
			builder.Append($".{propertyType.Name}");
		}

		if (isReadOnly)
		{
			builder.Append(".ReadOnly");
		}

		return builder.ToString();
	}
}