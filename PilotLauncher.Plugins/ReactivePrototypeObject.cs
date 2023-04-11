using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public abstract class ReactivePropertyInfo : ReactiveObject
{
	public PropertyInfo PropertyInfo { get; }
	public ReactivePrototypeObject SourceObject { get; }

	public bool CanEdit => PropertyInfo.CanWrite;

	protected ReactivePropertyInfo(ReactivePrototypeObject sourceObject, PropertyInfo propertyInfo, Type propertyType)
	{
		ArgumentNullException.ThrowIfNull(sourceObject);
		ArgumentNullException.ThrowIfNull(propertyInfo);
		ArgumentNullException.ThrowIfNull(propertyType);

		if (propertyInfo.PropertyType != propertyType)
			throw new ArgumentException($"property must be of type {propertyType.Name}");
		if (!propertyInfo.CanRead)
			throw new ArgumentException("property must be readable", nameof(propertyInfo));

		SourceObject = sourceObject;
		PropertyInfo = propertyInfo;
	}
}

public class ReactivePropertyInfo<T> : ReactivePropertyInfo
{
	public T Value
	{
		get => _value.Value;
		set => PropertyInfo.SetValue(SourceObject, value);
	}

	private readonly ObservableAsPropertyHelper<T> _value;

	public ReactivePropertyInfo(ReactivePrototypeObject sourceObject, PropertyInfo propertyInfo)
		: base(sourceObject, propertyInfo, typeof(T))
	{
		var parameter = Expression.Parameter(sourceObject.GetType(), "sourceObject");

		var expression = Expression.Lambda<Func<ReactivePrototypeObject, T>>(
			Expression.Property(
				parameter,
				propertyInfo
			),
			parameter
		);

		_value = sourceObject.WhenAnyValue(expression)
			.ToProperty(this, x => x.Value);
	}
}

[AttributeUsage(AttributeTargets.Property)]
public class ReactivePropertyInfoAttribute : Attribute
{
	
}

public class ReactivePrototypeObject : ReactiveObject
{
	public IEnumerable<ReactivePropertyInfo> GetExposedProperties()
	{
		return this.GetType().GetProperties()
			.Where(info => info.PropertyType == typeof(string))
			.Select(info => new ReactivePropertyInfo<string>(this, info));
	}
}