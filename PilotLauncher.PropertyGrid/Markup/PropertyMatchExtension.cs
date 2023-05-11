using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace PilotLauncher.PropertyGrid;

[MarkupExtensionReturnType(typeof(Func<PropertyGridItem, bool>))]
public class PropertyMatchExtension : MarkupExtension
{
	public bool? IsReadOnly { get; set; }

	public Type? ExactType { get; set; }
	public Type? BaseType { get; set; }
	public bool? IsValueType { get; set; }
	public bool? IsEnum { get; set; }

	private IEnumerable<Func<Type, bool>> GetTypeMatchConditions()
	{
		if (ExactType is not null) yield return type => type == ExactType;
		if (BaseType is not null) yield return type => type.IsAssignableTo(BaseType);
		if (IsValueType is not null) yield return type => type.IsValueType == IsValueType;
		if (IsEnum is not null) yield return type => type.IsEnum == IsEnum;
	}

	public IEnumerable<Func<PropertyGridItem, bool>> CreateItemMatchConditions()
	{
		if (IsReadOnly is not null) yield return item => item.IsReadOnly == IsReadOnly;
	}

	private Func<PropertyGridItem, bool> CreateItemMatch(
		IEnumerable<Func<PropertyGridItem, bool>> itemConditions,
		IEnumerable<Func<Type, bool>> typeConditions)
	{
		return item =>
		{
			var propertyType = item.PropertyInfo.PropertyType;

			return itemConditions.All(condition => condition(item))
				&& typeConditions.All(condition => condition(propertyType));
		};
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		return CreateItemMatch(CreateItemMatchConditions(), GetTypeMatchConditions());
	}
}