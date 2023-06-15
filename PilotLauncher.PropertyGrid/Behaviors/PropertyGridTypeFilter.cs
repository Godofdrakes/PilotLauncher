using System;
using System.Windows;
using PilotLauncher.WPF.Common;

namespace PilotLauncher.PropertyGrid;

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

	public Type? TargetType
	{
		get => (Type?)GetValue(TargetTypeProperty);
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