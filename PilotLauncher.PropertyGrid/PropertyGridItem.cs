using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid;

public sealed class PropertyGridItem : ReactiveObject, IDisposable
{
	public PropertyInfo PropertyInfo { get; }

	public bool IsReadOnly { get; }

	public int SortValue { get; }

	public string Category { get; }

	public string Template { get; }

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

	public PropertyGridItem(object propertySource, PropertyInfo propertyInfo)
	{
		PropertyInfo = propertyInfo;
		IsReadOnly = propertyInfo is not { CanWrite: true, SetMethod.IsPublic: true };
		SortValue = PropertyGridSortAttribute.GetValue(propertyInfo);
		Category = PropertyGridCategoryAttribute.GetValue(propertyInfo);
		Template = PropertyGridTemplateAttribute.GetValue(propertyInfo, IsReadOnly);

		var propertyExpression = Expression.Property(
			Expression.Parameter(propertySource.GetType(), nameof(propertySource)),
			propertyInfo);

		// Bind to updates in the source object, if any
		propertySource.WhenAnyDynamic(propertyExpression, change => change.GetValueOrDefault())
			.Subscribe(value => this.RaiseAndSetIfChanged(ref _value, value, nameof(Value)))
			.DisposeWith(_disposable);

		// Propagate changes back to the source, making sure not to recurse
		this.ObservableForProperty(item => item.Value, beforeChange: false, skipInitial: true)
			.Subscribe(change =>
			{
				using var suppress = SuppressChangeNotifications();
				propertyInfo.SetValue(propertySource, change.GetValue());
			})
			.DisposeWith(_disposable);
	}

	public static IEnumerable<PropertyGridItem> Scan(object? propertySource, Func<PropertyInfo, bool>? filter = default)
	{
		if (propertySource is null)
		{
			return Enumerable.Empty<PropertyGridItem>();
		}

		var properties = propertySource.GetType()
			// Only public non-static properties are supported
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			// None of this works if the property isn't readable
			.Where(info => info.CanRead);

		if (filter is not null)
		{
			properties = properties.Where(filter);
		}

		return properties.Select(info => new PropertyGridItem(propertySource, info));
	}

	public void Dispose() => _disposable.Dispose();
}