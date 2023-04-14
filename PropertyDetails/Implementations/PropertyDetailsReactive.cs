using System.Linq.Expressions;
using System.Reflection;
using PropertyDetails.Interfaces;
using ReactiveUI;

namespace PropertyDetails.Implementations;

internal class PropertyDetailsReactive : ReactiveObject, IPropertyDetails
{
	public string PropertyName { get; }
	public Type PropertyType { get; }
	public bool CanWrite { get; }

	public object? Value
	{
		get => _value.Value;
		set => _propertyInfo.SetValue(_sourceObject, value);
	}

	private readonly ReactiveObject _sourceObject;
	private readonly PropertyInfo _propertyInfo;
	private readonly ObservableAsPropertyHelper<object?> _value;

	public PropertyDetailsReactive(ReactiveObject sourceObject, PropertyInfo propertyInfo)
	{
		_sourceObject = sourceObject;
		_propertyInfo = propertyInfo;
		_value = sourceObject.WhenAnyDynamic(
				property1: Expression.Property(
					Expression.Parameter(sourceObject.GetType(), nameof(sourceObject)),
					propertyInfo
				),
				change => change.GetValue())
			.ToProperty(this, x => x.Value);
		
		PropertyName = _propertyInfo.GetPropertyName();
		PropertyType = _propertyInfo.PropertyType;
		CanWrite = _propertyInfo.GetPropertyCanWrite();
	}
}