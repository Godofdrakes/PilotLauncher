using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using PilotLauncher.WPF.Common;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid;

public delegate void PropertyGridItemAddedEventHandler(object sender, PropertyGridItemAddedEventArgs e);

public class PropertyGridItemAddedEventArgs : CancelEventArgs
{
	public PropertyGridItemAddedEventArgs(PropertyInfo propertyInfo)
	{
		PropertyInfo = propertyInfo;
	}

	public PropertyInfo PropertyInfo { get; }
}

public partial class PropertyGridView
{
#region Dependency Properties

	public static readonly DependencyProperty ShowPropertyNameProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridView view) => view.ShowPropertyName, defaultValue: true);

	public static readonly DependencyProperty ShowPropertyTypeProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridView view) => view.ShowPropertyType, defaultValue: true);

	public static readonly DependencyProperty PropertyTemplateSelectorProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridView view) => view.PropertyTemplateSelector,
			defaultValue: new PropertyGridTemplateSelector());

	public static readonly DependencyProperty PropertySourceProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridView view) => view.PropertySource, defaultValue: null);

#endregion

#region Public Properties

	public bool ShowPropertyName
	{
		get => (bool)GetValue(ShowPropertyNameProperty);
		set => SetValue(ShowPropertyNameProperty, value);
	}

	public bool ShowPropertyType
	{
		get => (bool)GetValue(ShowPropertyTypeProperty);
		set => SetValue(ShowPropertyTypeProperty, value);
	}

	public PropertyGridTemplateSelector? PropertyTemplateSelector
	{
		get => GetValue(PropertyTemplateSelectorProperty) as PropertyGridTemplateSelector;
		set => SetValue(PropertyTemplateSelectorProperty, value);
	}

	public object? PropertySource
	{
		get => GetValue(PropertySourceProperty);
		set => SetValue(PropertySourceProperty, value);
	}

	public IEnumerable<PropertyGridItem> PropertyItems { get; }
	public IEnumerable<IGroup<PropertyGridItem, string, string>> PropertyGroups { get; }

#endregion

#region Public Events

	public event PropertyGridItemAddedEventHandler? PropertyItemAdded;

#endregion

	public PropertyGridView()
	{
		InitializeComponent();

		this.WhenAnyValue(view => view.ShowPropertyName)
			.Select(show => show ? Visibility.Visible : Visibility.Collapsed)
			.BindTo(PropertyNameColumn, column => column.Visibility);
		this.WhenAnyValue(view => view.ShowPropertyType)
			.Select(show => show ? Visibility.Visible : Visibility.Collapsed)
			.BindTo(PropertyTypeColumn, column => column.Visibility);
		this.WhenAnyValue(view => view.PropertyTemplateSelector)
			.BindTo(PropertyValueColumn, column => column.CellTemplateSelector);

		this.WhenAnyValue(view => view.PropertySource)
			.Select(source => PropertyGridItem
				.Scan(source, FilterPropertyInfo)
				.AsObservableChangeSet(item => item.PropertyInfo.Name)
				// Dispose of property items when no longer in use
				.DisposeMany())
			// Throw out all existing items when a new property source is set
			.Switch()
			// Sort property items
			.SortBy(item => item.SortValue,
				sortOptimisations: SortOptimisations.ComparesImmutableValuesOnly)
			// Cache the property item list, in case we don't want groups for some reason
			.Bind(out var propertyItems)
			// Create the property groups
			.Group(item => item.Category)
			// Sort the property groups
			.SortBy(group => group.Key,
				sortOptimisations: SortOptimisations.ComparesImmutableValuesOnly)
			// Cache the property group list separately
			.Bind(out var propertyGroups)
			.Subscribe();

		PropertyItems = propertyItems;
		PropertyGroups = propertyGroups;
	}

	private bool FilterPropertyInfo(PropertyInfo propertyInfo)
	{
		if (PropertyItemAdded is null)
		{
			return true;
		}

		var eventArgs = new PropertyGridItemAddedEventArgs(propertyInfo);
		PropertyItemAdded.Invoke(this, eventArgs);
		return !eventArgs.Cancel;
	}
}