using System;
using System.Collections.Generic;
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

[TemplatePart(Name = nameof(PropertyNameColumn), Type = typeof(DataGridColumn))]
[TemplatePart(Name = nameof(PropertyTypeColumn), Type = typeof(DataGridColumn))]
[TemplatePart(Name = nameof(PropertyValueColumn), Type = typeof(DataGridTemplateColumn))]
public class PropertyGrid : Control
{
	[DebuggerHidden, StackTraceHidden]
	private static void ArgumentAssertion<T>(
		T param,
		Expression<Func<T, bool>> assertion,
		string? message = default,
		[CallerArgumentExpression("param")] string paramName = "")
	{
		if (!assertion.Compile().Invoke(param))
		{
			ThrowArgumentAssertion(message, assertion, paramName);
		}
	}

	[DoesNotReturn, DebuggerHidden, StackTraceHidden]
	private static void ThrowArgumentAssertion(string? message, LambdaExpression assertion, string paramName)
	{
		throw new ArgumentException(message ?? $"Assertion failed: {assertion.Body}", paramName);
	}

	private static PropertyMetadata? CreatePropertyMetadata(
		object? defaultValue = default,
		bool affectsRender = default,
		bool affectsMeasure = default,
		bool affectsArrange = default)
	{
		FrameworkPropertyMetadata? frameworkMetadata = default;

		if (affectsRender)
		{
			frameworkMetadata ??= new FrameworkPropertyMetadata();
			frameworkMetadata.AffectsRender = true;
		}

		if (affectsMeasure)
		{
			frameworkMetadata ??= new FrameworkPropertyMetadata();
			frameworkMetadata.AffectsMeasure = true;
		}

		if (affectsArrange)
		{
			frameworkMetadata ??= new FrameworkPropertyMetadata();
			frameworkMetadata.AffectsArrange = true;
		}

		PropertyMetadata? propertyMetadata = frameworkMetadata;

		if (defaultValue is not null)
		{
			propertyMetadata ??= new PropertyMetadata();
			propertyMetadata.DefaultValue = defaultValue;
		}

		return propertyMetadata;
	}

	private static TReturn RegisterPropertyCommon<TProperty, TReturn>(
		Func<string, Type, Type, PropertyMetadata?, ValidateValueCallback?, TReturn> registerFunc,
		Expression<Func<PropertyGrid, TProperty>> propertyExpression,
		PropertyMetadata? propertyMetadata = default,
		ValidateValueCallback? validateValueCallback = default)
	{
		ArgumentNullException.ThrowIfNull(registerFunc);
		ArgumentNullException.ThrowIfNull(propertyExpression);

		var expression = Reflection.Rewrite(propertyExpression.Body);

		var parent = expression.GetParent();

		ArgumentAssertion(parent, expr => expr != null,
			"The property expression does not have a valid parent.");
		ArgumentAssertion(parent, expr => expr!.NodeType == ExpressionType.Parameter,
			"Property expression must be of the form 'x => x.SomeProperty'");

		var memberInfo = expression!.GetMemberInfo();

		ArgumentAssertion(memberInfo, expr => expr != null,
			"The property expression does not point towards a valid member.");

		var memberName = memberInfo!.Name;
		if (expression is IndexExpression)
		{
			memberName += "[]";
		}

		return registerFunc.Invoke(
			memberName,
			typeof(TProperty),
			typeof(PropertyGrid),
			propertyMetadata,
			validateValueCallback);
	}

	private static DependencyProperty RegisterProperty<TProperty>(
		Expression<Func<PropertyGrid, TProperty>> propertyExpression,
		TProperty defaultValue = default!,
		bool affectsRender = default,
		bool affectsMeasure = default,
		bool affectsArrange = default,
		ValidateValueCallback? validateValueCallback = default) =>
		RegisterPropertyCommon(
			DependencyProperty.Register,
			propertyExpression,
			CreatePropertyMetadata(defaultValue, affectsRender, affectsMeasure, affectsArrange),
			validateValueCallback);

	private static DependencyPropertyKey RegisterReadOnlyProperty<TProperty>(
		Expression<Func<PropertyGrid, TProperty>> propertyExpression,
		TProperty defaultValue = default!,
		bool affectsRender = default,
		bool affectsMeasure = default,
		bool affectsArrange = default,
		ValidateValueCallback? validateValueCallback = default) =>
		RegisterPropertyCommon(
			DependencyProperty.RegisterReadOnly,
			propertyExpression,
			CreatePropertyMetadata(defaultValue, affectsRender, affectsMeasure, affectsArrange),
			validateValueCallback);

	public static readonly DependencyProperty PropertySourceProperty =
		RegisterProperty(grid => grid.PropertySource);

	private static readonly DependencyPropertyKey PropertyItemsPropertyKey =
		RegisterReadOnlyProperty(grid => grid.PropertyItems);

	public static readonly DependencyProperty PropertyItemsProperty =
		PropertyItemsPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PropertyEditTemplateSelectorProperty =
		RegisterProperty(grid => grid.PropertyEditTemplateSelector);

	public static readonly DependencyProperty PropertyNameVisibilityProperty =
		RegisterProperty(grid => grid.PropertyNameVisibility);

	public static readonly DependencyProperty PropertyTypeVisibilityProperty =
		RegisterProperty(grid => grid.PropertyTypeVisibility);

	private static readonly DependencyPropertyKey PropertyNameColumnPropertyKey =
		RegisterReadOnlyProperty(grid => grid.PropertyNameColumn);

	private static readonly DependencyPropertyKey PropertyTypeColumnPropertyKey =
		RegisterReadOnlyProperty(grid => grid.PropertyTypeColumn);

	private static readonly DependencyPropertyKey PropertyValueColumnPropertyKey =
		RegisterReadOnlyProperty(grid => grid.PropertyValueColumn);

	public static readonly DependencyProperty PropertyNameColumnProperty =
		PropertyNameColumnPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PropertyTypeColumnProperty =
		PropertyTypeColumnPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PropertyValueColumnProperty =
		PropertyValueColumnPropertyKey.DependencyProperty;

	static PropertyGrid()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid),
			new FrameworkPropertyMetadata(typeof(PropertyGrid)));
	}

	public PropertyGrid()
	{
		this.WhenAnyValue(grid => grid.PropertyNameColumn, grid => grid.PropertyNameVisibility)
			.SubscribeOn(Dispatcher)
			.ObserveOn(Dispatcher)
			.Subscribe(pair =>
			{
				if (pair.Item1 is not null)
				{
					pair.Item1.Visibility = pair.Item2;
				}
			});

		this.WhenAnyValue(grid => grid.PropertyTypeColumn, grid => grid.PropertyTypeVisibility)
			.SubscribeOn(Dispatcher)
			.ObserveOn(Dispatcher)
			.Subscribe(pair =>
			{
				if (pair.Item1 is not null)
				{
					pair.Item1.Visibility = pair.Item2;
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
			.Bind(out var propertyItems)
			.Subscribe();

		PropertyItems = propertyItems;
	}

	public object? PropertySource
	{
		get => GetValue(PropertySourceProperty);
		set => SetValue(PropertySourceProperty, value);
	}

	public IEnumerable<PropertyGridItem> PropertyItems
	{
		get => (IEnumerable<PropertyGridItem>) GetValue(PropertyItemsProperty);
		private set => SetValue(PropertyItemsPropertyKey, value);
	}

	public DataTemplateSelector? PropertyEditTemplateSelector
	{
		get => (DataTemplateSelector) GetValue(PropertyEditTemplateSelectorProperty);
		set => SetValue(PropertyEditTemplateSelectorProperty, value);
	}

	public Visibility PropertyNameVisibility
	{
		get => (Visibility) GetValue(PropertyNameVisibilityProperty);
		set => SetValue(PropertyNameVisibilityProperty, value);
	}

	public Visibility PropertyTypeVisibility
	{
		get => (Visibility) GetValue(PropertyTypeVisibilityProperty);
		set => SetValue(PropertyTypeVisibilityProperty, value);
	}

	public event PropertyGridItemAddedEventHandler? PropertyItemAdded;

	private bool FilterPropertyInfo(PropertyInfo propertyInfo)
	{
		var eventArgs = new PropertyGridItemAddedEventArgs(propertyInfo);
		PropertyItemAdded?.Invoke(this, eventArgs);
		return !eventArgs.Cancel;
	}

	public DataGridColumn? PropertyNameColumn
	{
		get => (DataGridColumn) GetValue(PropertyNameColumnProperty);
		private set => SetValue(PropertyNameColumnPropertyKey, value);
	}

	public DataGridColumn? PropertyTypeColumn
	{
		get => (DataGridColumn) GetValue(PropertyTypeColumnProperty);
		private set => SetValue(PropertyTypeColumnPropertyKey, value);
	}

	public DataGridTemplateColumn? PropertyValueColumn
	{
		get => (DataGridTemplateColumn) GetValue(PropertyValueColumnProperty);
		private set => SetValue(PropertyValueColumnPropertyKey, value);
	}

	public override void OnApplyTemplate()
	{
		PropertyNameColumn = GetTemplateChild(nameof(PropertyNameColumn)) as DataGridColumn;
		PropertyTypeColumn = GetTemplateChild(nameof(PropertyTypeColumn)) as DataGridColumn;
		PropertyValueColumn = GetTemplateChild(nameof(PropertyValueColumn)) as DataGridTemplateColumn;
	}
}

public delegate void PropertyGridItemAddedEventHandler(object sender, PropertyGridItemAddedEventArgs e);

public class PropertyGridItemAddedEventArgs : CancelEventArgs
{
	public PropertyInfo PropertyInfo { get; }

	public PropertyGridItemAddedEventArgs(PropertyInfo propertyInfo)
	{
		PropertyInfo = propertyInfo;
	}
}