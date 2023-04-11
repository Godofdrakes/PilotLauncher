using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public class ReactivePropertyInfo : ReactiveObject
{
	public PropertyInfo PropertyInfo { get; }
	public ReactivePrototypeObject SourceObject { get; }

	public object? Value
	{
		get => _value.Value;
		set => PropertyInfo.SetValue(SourceObject, value);
	}

	private readonly ObservableAsPropertyHelper<object?> _value;

	public ReactivePropertyInfo(ReactivePrototypeObject sourceObject, PropertyInfo propertyInfo)
	{
		ArgumentNullException.ThrowIfNull(sourceObject);
		ArgumentNullException.ThrowIfNull(propertyInfo);

		if (!propertyInfo.CanRead)
			throw new ArgumentException("property must be readable", nameof(propertyInfo));

		SourceObject = sourceObject;
		PropertyInfo = propertyInfo;

		_value = sourceObject.WhenAnyDynamic(
				property1: Expression.Property(
					Expression.Parameter(sourceObject.GetType(), nameof(sourceObject)),
					propertyInfo
				),
				change => change.GetValue())
			.ToProperty(this, x => x.Value);
	}
}

[AttributeUsage(AttributeTargets.Property)]
public class ReactivePropertyInfoAttribute : Attribute { }

public class ReactivePrototypeObject : ReactiveObject
{
	public IEnumerable<ReactivePropertyInfo> GetExposedProperties() => GetType().GetProperties()
		.Where(info => info.GetCustomAttribute<ReactivePropertyInfoAttribute>() is not null)
		.Select(info => new ReactivePropertyInfo(this, info));
}