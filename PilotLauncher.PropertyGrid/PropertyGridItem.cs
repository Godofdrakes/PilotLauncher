using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PilotLauncher.PropertyGrid;

public sealed class PropertyGridItem : ReactiveObject, IDisposable
{
	public string PropertyName { get; }
	public Type PropertyType { get; }
	public bool IsNullable { get; }
	public bool IsReadOnly { get; }

	public object? Value
	{
		get => _value;
		set
		{
			if (IsReadOnly)
				throw new InvalidOperationException($"Property {PropertyName} is read only");

			this.RaiseAndSetIfChanged(ref _value, value);
		}
	}

	private object? _value;

	public string TextValue => _textValue.Value;
	private readonly ObservableAsPropertyHelper<string> _textValue;

	private readonly CompositeDisposable _disposable = new();

	private readonly ManualResetEventSlim _suppressValuePropagation = new(false);

	public PropertyGridItem(object propertySource, PropertyInfo propertyInfo)
	{
		var nullabilityInfo = NullabilityContext.Create(propertyInfo);

		PropertyName = propertyInfo.Name;
		PropertyType = propertyInfo.PropertyType;
		IsNullable = nullabilityInfo.WriteState is not NullabilityState.NotNull;
		IsReadOnly = propertyInfo is not { CanWrite: true, SetMethod.IsPublic: true };

		var propertyExpression = Expression.Property(
			Expression.Parameter(propertySource.GetType(), nameof(propertySource)),
			propertyInfo);

		// Bind to updates in the source object.
		// If the object doesn't support property changed notifications this will only ever occur once.
		propertySource.WhenAnyDynamic(propertyExpression, change => change.GetValueOrDefault())
			.Subscribe(value =>
			{
				// Avoid infinite loop
				_suppressValuePropagation.Set();
				this.RaiseAndSetIfChanged(ref _value, value, nameof(Value));
				_suppressValuePropagation.Reset();
			})
			.DisposeWith(_disposable);

		// Update textual representation of value
		_textValue = this.WhenAnyValue(instance => instance.Value)
			.Select(value => value?.ToString() ?? "<null>")
			.ToProperty(this, instance => instance.TextValue)
			.DisposeWith(_disposable);

		// Propagate changes back to the source
		this.ObservableForProperty(item => item.Value, beforeChange: false, skipInitial: true)
			.SkipWhile(_ => _suppressValuePropagation.IsSet)
			.Subscribe(change => propertyInfo.SetValue(propertySource, change.GetValue()))
			.DisposeWith(_disposable);
	}

	private static readonly NullabilityInfoContext NullabilityContext = new();

	public static IEnumerable<PropertyInfo> Scan(object propertySource) => propertySource.GetType()
		.GetProperties(BindingFlags.Public | BindingFlags.Instance)
		.Where(info => info.CanRead);

	public void Dispose() => _disposable.Dispose();
}