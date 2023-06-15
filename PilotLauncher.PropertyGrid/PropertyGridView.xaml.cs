using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;
using DynamicData;
using DynamicData.Binding;
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

public abstract class PropertyGridFilter : Behavior<PropertyGridView>
{
	protected override void OnAttached() => AssociatedObject.PropertyItemAdded += OnPropertyItemAdded;
	protected override void OnDetaching() => AssociatedObject.PropertyItemAdded -= OnPropertyItemAdded;

	protected abstract void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e);
}

// Exclude properties declared in the specified assembly
public class PropertyGridDeclaringAssemblyFilter : PropertyGridFilter
{
	public static readonly DependencyProperty TargetAssemblyProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridDeclaringAssemblyFilter filter) => filter.TargetAssembly);

	public string TargetAssembly
	{
		get => (string)GetValue(TargetAssemblyProperty);
		set => SetValue(TargetAssemblyProperty, value);
	}

	protected override void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		if (string.IsNullOrEmpty(TargetAssembly))
			throw new InvalidOperationException("Invalid TargetAssembly");

		var declaringType = e.PropertyInfo.DeclaringType;
		var assemblyName = declaringType?.Assembly.GetName();
		if (assemblyName?.Name?.StartsWith(TargetAssembly) is true)
		{
			e.Cancel = true;
		}
	}
}

public abstract class PropertyGridTypeFilter : PropertyGridFilter
{
	public static readonly DependencyProperty TargetTypeProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridDeclaringTypeFilter filter) => filter.TargetType);

	public static readonly DependencyProperty FilterAssignableToProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridDeclaringTypeFilter filter) => filter.FilterAssignableTo,
			defaultValue: false);

	public static readonly DependencyProperty FilterAssignableFromProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridDeclaringTypeFilter filter) => filter.FilterAssignableFrom,
			defaultValue: true);

	public Type TargetType
	{
		get => (Type)GetValue(TargetTypeProperty);
		set => SetValue(TargetTypeProperty, value);
	}

	public bool FilterAssignableTo
	{
		get => (bool)GetValue(FilterAssignableToProperty);
		set => SetValue(FilterAssignableToProperty, value);
	}

	public bool FilterAssignableFrom
	{
		get => (bool)GetValue(FilterAssignableFromProperty);
		set => SetValue(FilterAssignableFromProperty, value);
	}
}

// Exclude properties declared in the specified type
public class PropertyGridDeclaringTypeFilter : PropertyGridTypeFilter
{
	protected override void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		if (TargetType is null)
			throw new InvalidOperationException("Invalid TargetType");

		var declaringType = e.PropertyInfo.DeclaringType;

		if (declaringType is null)
		{
			return;
		}

		if (declaringType == TargetType
			|| (FilterAssignableTo && declaringType.IsAssignableTo(TargetType))
			|| (FilterAssignableFrom && declaringType.IsAssignableFrom(TargetType)))
		{
			e.Cancel = true;
		}
	}
}

// Exclude properties of the specified type
public class PropertyGridPropertyTypeFilter : PropertyGridTypeFilter
{
	protected override void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		if (TargetType is null)
			throw new InvalidOperationException("PropertyGridPropertyTypeFilter: Invalid TargetType");

		var propertyType = e.PropertyInfo.PropertyType;

		if (propertyType == TargetType
			|| (FilterAssignableTo && propertyType.IsAssignableTo(TargetType))
			|| (FilterAssignableFrom && propertyType.IsAssignableFrom(TargetType)))
		{
			e.Cancel = true;
		}
	}
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
				.DisposeMany())
			.Switch()
			.Bind(out var propertyItems)
			.Subscribe();

		PropertyItems = propertyItems;
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