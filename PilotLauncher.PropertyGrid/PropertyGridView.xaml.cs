using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid;

public partial class PropertyGridView
{
#region Dependency Properties

	public static readonly DependencyProperty ShowPropertyNameProperty =
		DependencyObjectEx.RegisterProperty((PropertyGridView view) => view.ShowPropertyName,
			defaultValue: true);

	public static readonly DependencyProperty ShowPropertyTypeProperty =
		DependencyObjectEx.RegisterProperty((PropertyGridView view) => view.ShowPropertyType,
			defaultValue: true);

	public static readonly DependencyProperty PropertyTemplateSelectorProperty =
		DependencyObjectEx.RegisterProperty((PropertyGridView view) => view.PropertyTemplateSelector,
			defaultValue: new PropertyGridTemplateSelector());

	public static readonly DependencyProperty PropertySourceProperty =
		DependencyObjectEx.RegisterProperty((PropertyGridView view) => view.PropertySource,
			defaultValue: null);

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

#endregion

#region Public Events

	public event PropertyGridItemAddedEventHandler? PropertyItemAdded;

#endregion

	public PropertyGridView()
	{
		InitializeComponent();

		var propertyNameColumn = CreatePropertyNameColumn();
		var propertyTypeColumn = CreatePropertyTypeColumn();
		var propertyValueColumn = CreatePropertyValueColumn();

		this.WhenAnyValue(view => view.ShowPropertyName)
			.Select(show => show ? Visibility.Visible : Visibility.Collapsed)
			.BindTo(propertyNameColumn, column => column.Visibility);
		this.WhenAnyValue(view => view.ShowPropertyType)
			.Select(show => show ? Visibility.Visible : Visibility.Collapsed)
			.BindTo(propertyTypeColumn, column => column.Visibility);
		this.WhenAnyValue(view => view.PropertyTemplateSelector)
			.BindTo(propertyValueColumn, column => column.CellTemplateSelector);

		this.WhenAnyValue(view => view.PropertySource)
			.Select(source => PropertyGridItem
				.Scan(source, FilterPropertyInfo)
				.AsObservableChangeSet(item => item.PropertyInfo.Name)
				.DisposeMany())
			.Switch()
			.SubscribeOn(Dispatcher)
			.ObserveOn(Dispatcher)
			.Bind(out var propertyItems)
			.Subscribe();

		DataGrid.Columns.Add(propertyNameColumn);
		DataGrid.Columns.Add(propertyTypeColumn);
		DataGrid.Columns.Add(propertyValueColumn);
		DataGrid.ItemsSource = propertyItems;
	}

	private bool FilterPropertyInfo(PropertyInfo propertyInfo)
	{
		var eventArgs = new PropertyGridItemAddedEventArgs(propertyInfo);
		PropertyItemAdded?.Invoke(this, eventArgs);
		return !eventArgs.Cancel;
	}

	private static DataGridTextColumn CreatePropertyNameColumn() => new()
	{
		Header = "Name",
		Width = DataGridLength.Auto,
		Binding = BindingEx.Create((PropertyGridItem item) =>
			item.PropertyInfo.Name),
	};

	private static DataGridTextColumn CreatePropertyTypeColumn() => new()
	{
		Header = "Type",
		Width = DataGridLength.SizeToCells,
		Binding = BindingEx.Create((PropertyGridItem item) =>
			item.PropertyInfo.PropertyType.Name),
	};

	private static DataGridTemplateColumn CreatePropertyValueColumn() => new()
	{
		Header = "Value",
		Width = new DataGridLength(1, DataGridLengthUnitType.Star),
	};
}