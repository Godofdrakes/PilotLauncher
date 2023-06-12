using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid;

public sealed class PropertyGridItem : ReactiveObject, IDisposable
{
	public PropertyInfo PropertyInfo { get; }

	public bool IsReadOnly { get; }

	public int SortValue { get; }

	public string Category { get; }

	public object? Value
	{
		get => _value;
		set
		{
			if (IsReadOnly)
				throw new InvalidOperationException($"Property {PropertyInfo.Name} is read only");

			this.RaiseAndSetIfChanged(ref _value, value);
		}
	}

	private object? _value;

	private readonly CompositeDisposable _disposable = new();

	private readonly ManualResetEventSlim _suppressValuePropagation = new(false);

	public PropertyGridItem(object propertySource, PropertyInfo propertyInfo)
	{
		PropertyInfo = propertyInfo;
		IsReadOnly = propertyInfo is not { CanWrite: true, SetMethod.IsPublic: true };
		SortValue = PropertyGridSortAttribute.GetValue(propertyInfo);
		Category = PropertyGridCategoryAttribute.GetValue(propertyInfo);

		var propertyExpression = Expression.Property(
			Expression.Parameter(propertySource.GetType(), nameof(propertySource)),
			propertyInfo);

		// Bind to updates in the source object.
		// If the object doesn't support property changed notifications this will only ever occur once.
		propertySource.WhenAnyDynamic(propertyExpression, change => change.GetValueOrDefault())
			.Subscribe(value =>
			{
				// Avoid infinite loop
				using var scopedSuppression = _suppressValuePropagation.ScopedSet();
				this.RaiseAndSetIfChanged(ref _value, value, nameof(Value));
			})
			.DisposeWith(_disposable);

		// Propagate changes back to the source
		this.ObservableForProperty(item => item.Value, beforeChange: false, skipInitial: true)
			.SkipWhile(_ => _suppressValuePropagation.IsSet)
			.Subscribe(change => propertyInfo.SetValue(propertySource, change.GetValue()))
			.DisposeWith(_disposable);
	}

	public static IEnumerable<PropertyGridItem> Scan(object? propertySource, Func<PropertyInfo, bool>? filter = default)
	{
		if (propertySource is null)
		{
			return Enumerable.Empty<PropertyGridItem>();
		}

		var properties = propertySource.GetType()
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(info => info.CanRead);

		if (filter is not null)
		{
			properties = properties.Where(filter);
		}

		return properties.Select(info => new PropertyGridItem(propertySource!, info));
	}

	public void Dispose() => _disposable.Dispose();
}