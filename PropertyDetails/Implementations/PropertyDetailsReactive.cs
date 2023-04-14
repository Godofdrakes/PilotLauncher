using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using DynamicData.Binding;
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
	private readonly ObservableAsPropertyHelper<string> _valueDisplay;

	public PropertyDetailsReactive(ReactiveObject sourceObject, PropertyInfo propertyInfo)
	{
		var valueObservable = sourceObject.WhenAnyDynamic(
			property1: Expression.Property(
				Expression.Parameter(sourceObject.GetType(), nameof(sourceObject)),
				propertyInfo
			),
			change => change.GetValue());

		_sourceObject = sourceObject;
		_propertyInfo = propertyInfo;
		_value = valueObservable.ToProperty(this, x => x.Value);
		
		PropertyName = _propertyInfo.GetPropertyName();
		PropertyType = _propertyInfo.PropertyType;
		CanWrite = _propertyInfo.GetPropertyCanWrite();
	}
}