using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid;

public delegate void PropertyGridItemAddedEventHandler(object sender, PropertyGridItemAddedEventArgs e);

public class PropertyGridItemAddedEventArgs : CancelEventArgs
{
	public PropertyInfo PropertyInfo { get; }

	public PropertyGridItemAddedEventArgs(PropertyInfo propertyInfo)
	{
		PropertyInfo = propertyInfo;
	}
}

[TemplatePart(Name = nameof(PropertyNameColumn), Type = typeof(DataGridColumn))]
[TemplatePart(Name = nameof(PropertyTypeColumn), Type = typeof(DataGridColumn))]
[TemplatePart(Name = nameof(PropertyValueColumn), Type = typeof(DataGridTemplateColumn))]
public class PropertyGrid : Control
{
#region Dependency Properties

	public static readonly DependencyProperty PropertySourceProperty =
		DependencyObjectEx.RegisterProperty((PropertyGrid grid) => grid.PropertySource);

	public static readonly DependencyProperty PropertyTemplateSelectorProperty =
		DependencyObjectEx.RegisterProperty((PropertyGrid grid) => grid.PropertyTemplateSelector);

	public static readonly DependencyProperty PropertyNameVisibilityProperty =
		DependencyObjectEx.RegisterProperty((PropertyGrid grid) => grid.PropertyNameVisibility);

	public static readonly DependencyProperty PropertyTypeVisibilityProperty =
		DependencyObjectEx.RegisterProperty((PropertyGrid grid) => grid.PropertyTypeVisibility);

	private static readonly DependencyPropertyKey PropertyNameColumnPropertyKey =
		DependencyObjectEx.RegisterReadOnlyProperty((PropertyGrid grid) => grid.PropertyNameColumn);

	private static readonly DependencyPropertyKey PropertyTypeColumnPropertyKey =
		DependencyObjectEx.RegisterReadOnlyProperty((PropertyGrid grid) => grid.PropertyTypeColumn);

	private static readonly DependencyPropertyKey PropertyValueColumnPropertyKey =
		DependencyObjectEx.RegisterReadOnlyProperty((PropertyGrid grid) => grid.PropertyValueColumn);

	public static readonly DependencyProperty PropertyNameColumnProperty =
		PropertyNameColumnPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PropertyTypeColumnProperty =
		PropertyTypeColumnPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PropertyValueColumnProperty =
		PropertyValueColumnPropertyKey.DependencyProperty;

#endregion

#region Constructors

	static PropertyGrid()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid),
			new FrameworkPropertyMetadata(typeof(PropertyGrid)));
	}

	public PropertyGrid()
	{
		var nameVisibilityChanged = this.WhenAnyValue(grid => grid.PropertyNameColumn, grid => grid.PropertyNameVisibility);
		var typeVisibilityChanged = this.WhenAnyValue(grid => grid.PropertyTypeColumn, grid => grid.PropertyTypeVisibility);

		nameVisibilityChanged
			.Merge(typeVisibilityChanged)
			.SubscribeOn(Dispatcher)
			.ObserveOn(Dispatcher)
			.Subscribe(pair =>
			{
				if (pair.Item1 is not null)
				{
					pair.Item1.Visibility = pair.Item2;
				}
			});

		this.WhenAnyValue(grid => grid.PropertyValueColumn, grid => grid.PropertyTemplateSelector)
			.SubscribeOn(Dispatcher)
			.ObserveOn(Dispatcher)
			.Subscribe(pair =>
			{
				if (pair.Item1 is not null)
				{
					pair.Item1.CellTemplateSelector = pair.Item2;
				}
			});

		this.WhenAnyValue(propertyGrid => propertyGrid.PropertySource)
			.Select(propertySource => PropertyGridItem
				.Scan(propertySource, FilterPropertyInfo)
				.AsObservableChangeSet(item => item.PropertyInfo.Name)
				// Dispose of items when change set is disposed
				.DisposeMany())
			// Dispose of old changeset when new one is generated
			.Switch()
			.SubscribeOn(Dispatcher)
			.ObserveOn(Dispatcher)
			.Bind(out _propertyItems)
			.Subscribe();
	}

#endregion

#region Properties

	public object? PropertySource
	{
		get => GetValue(PropertySourceProperty);
		set => SetValue(PropertySourceProperty, value);
	}

	public IEnumerable<PropertyGridItem> PropertyItems => _propertyItems;
	private readonly ReadOnlyObservableCollection<PropertyGridItem> _propertyItems;

	public DataTemplateSelector? PropertyTemplateSelector
	{
		get => (DataTemplateSelector)GetValue(PropertyTemplateSelectorProperty);
		set => SetValue(PropertyTemplateSelectorProperty, value);
	}

	public Visibility PropertyNameVisibility
	{
		get => (Visibility)GetValue(PropertyNameVisibilityProperty);
		set => SetValue(PropertyNameVisibilityProperty, value);
	}

	public Visibility PropertyTypeVisibility
	{
		get => (Visibility)GetValue(PropertyTypeVisibilityProperty);
		set => SetValue(PropertyTypeVisibilityProperty, value);
	}

	public DataGridColumn? PropertyNameColumn
	{
		get => (DataGridColumn)GetValue(PropertyNameColumnProperty);
		private set => SetValue(PropertyNameColumnPropertyKey, value);
	}

	public DataGridColumn? PropertyTypeColumn
	{
		get => (DataGridColumn)GetValue(PropertyTypeColumnProperty);
		private set => SetValue(PropertyTypeColumnPropertyKey, value);
	}

	public DataGridTemplateColumn? PropertyValueColumn
	{
		get => (DataGridTemplateColumn)GetValue(PropertyValueColumnProperty);
		private set => SetValue(PropertyValueColumnPropertyKey, value);
	}

#endregion

#region Events

	public event PropertyGridItemAddedEventHandler? PropertyItemAdded;

#endregion

#region Methods

	private bool FilterPropertyInfo(PropertyInfo propertyInfo)
	{
		var eventArgs = new PropertyGridItemAddedEventArgs(propertyInfo);
		PropertyItemAdded?.Invoke(this, eventArgs);
		return !eventArgs.Cancel;
	}

	public override void OnApplyTemplate()
	{
		PropertyNameColumn = GetTemplateChild(nameof(PropertyNameColumn)) as DataGridColumn;
		PropertyTypeColumn = GetTemplateChild(nameof(PropertyTypeColumn)) as DataGridColumn;
		PropertyValueColumn = GetTemplateChild(nameof(PropertyValueColumn)) as DataGridTemplateColumn;
	}

#endregion
}