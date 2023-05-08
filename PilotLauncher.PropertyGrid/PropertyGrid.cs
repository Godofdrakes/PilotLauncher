using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid;

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

	private static readonly DependencyPropertyKey PropertyItemsPropertyKey =
		RegisterReadOnlyProperty(grid => grid.PropertyItems);

	public static readonly DependencyProperty PropertySourceProperty =
		RegisterProperty(grid => grid.PropertySource, affectsRender: true);

	public static readonly DependencyProperty DataTemplateSelectorProperty =
		RegisterProperty(grid => grid.DataTemplateSelector, affectsRender: true);

	public static readonly DependencyProperty PropertyItemsProperty =
		PropertyItemsPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PropertyNameVisibleProperty =
		RegisterProperty(grid => grid.PropertyNameVisible, defaultValue: true, affectsMeasure: true);

	public static readonly DependencyProperty PropertyTypeVisibleProperty =
		RegisterProperty(grid => grid.PropertyTypeVisible, defaultValue: false, affectsMeasure: true);

	static PropertyGrid()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid),
			new FrameworkPropertyMetadata(typeof(PropertyGrid)));
	}

	private static void DataTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		throw new System.NotImplementedException();
	}

	public PropertyGrid()
	{
		var propertySourceChanged = this
			.WhenAnyValue(propertyGrid => propertyGrid.PropertySource);

		var dataTemplateSelectorChanged = this
			.WhenAnyValue(propertyGrid => propertyGrid.DataTemplateSelector);

		var sourcePropertyChanged = propertySourceChanged
			.Select(sourceObject =>
			{
				if (sourceObject is not INotifyPropertyChanged notifyPropertyChanged)
				{
					return Observable.Never<EventPattern<PropertyChangedEventArgs>>();
				}

				return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
					handler => notifyPropertyChanged.PropertyChanged += handler,
					handler => notifyPropertyChanged.PropertyChanged -= handler);
			})
			.Switch();

		propertySourceChanged
			.Select(propertySource =>
			{
				var propertyEnumerable = propertySource is not null
					? PropertyGridItem.Scan(propertySource)
					: Enumerable.Empty<PropertyInfo>();

				return propertyEnumerable
					.Where(FilterPropertyInfo)
					// If there are any items to run select on propertySource won't be null
					.Select(info => new PropertyGridItem(propertySource!, info))
					.AsObservableChangeSet(item => item.PropertyName)
					// Dispose of items when change set is disposed
					.DisposeMany();
			})
			// Dispose of old changeset when new one is generated
			.Switch()
			.ToCollection()
			.SubscribeOn(Dispatcher)
			.Subscribe(items => PropertyItems = items);
	}

	public object? PropertySource
	{
		get => GetValue(PropertySourceProperty);
		set => SetValue(PropertySourceProperty, value);
	}

	public IReadOnlyCollection<PropertyGridItem>? PropertyItems
	{
		get => (IReadOnlyCollection<PropertyGridItem>)GetValue(PropertyItemsProperty);
		private set => SetValue(PropertyItemsPropertyKey, value);
	}

	public DataTemplateSelector? DataTemplateSelector
	{
		get => (DataTemplateSelector)GetValue(DataTemplateSelectorProperty);
		set => SetValue(DataTemplateSelectorProperty, value);
	}

	public bool PropertyNameVisible
	{
		get => (bool)GetValue(PropertyNameVisibleProperty);
		set => SetValue(PropertyNameVisibleProperty, value);
	}

	public bool PropertyTypeVisible
	{
		get => (bool)GetValue(PropertyTypeVisibleProperty);
		set => SetValue(PropertyTypeVisibleProperty, value);
	}

	public event PropertyGridItemAddedEventHandler? PropertyItemAdded;

	private bool FilterPropertyInfo(PropertyInfo propertyInfo)
	{
		var eventArgs = new PropertyGridItemAddedEventArgs(propertyInfo);
		PropertyItemAdded?.Invoke(this, eventArgs);
		return !eventArgs.Cancel;
	}

	public ItemsControl? ItemsControlElement
	{
		get => _itemsControlElement;
		set => _itemsControlElement = value;
	}

	private ItemsControl? _itemsControlElement;
}

public delegate void PropertyGridItemAddedEventHandler(object sender, PropertyGridItemAddedEventArgs eventArgs);

public class PropertyGridItemAddedEventArgs : CancelEventArgs
{
	public PropertyInfo PropertyInfo { get; }

	public PropertyGridItemAddedEventArgs(PropertyInfo propertyInfo)
	{
		PropertyInfo = propertyInfo;
	}
}