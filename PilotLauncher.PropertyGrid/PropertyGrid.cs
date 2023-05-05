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

	private static DependencyProperty RegisterProperty<T>(Expression<Func<PropertyGrid, T>> propertyExpression,
		PropertyMetadata? propertyMetadata = default, ValidateValueCallback? validateValueCallback = default) =>
		RegisterPropertyCommon(DependencyProperty.Register, propertyExpression, propertyMetadata,
			validateValueCallback);

	private static DependencyPropertyKey RegisterReadOnlyProperty<T>(
		Expression<Func<PropertyGrid, T>> propertyExpression, PropertyMetadata? propertyMetadata = default,
		ValidateValueCallback? validateValueCallback = default) =>
		RegisterPropertyCommon(DependencyProperty.RegisterReadOnly, propertyExpression, propertyMetadata,
			validateValueCallback);

	public static readonly DependencyProperty PropertySourceProperty = RegisterProperty(
		propertyGrid => propertyGrid.PropertySource, new FrameworkPropertyMetadata()
		{
			DefaultValue = null,
			AffectsRender = true,
			AffectsMeasure = true,
		});

	public static readonly DependencyProperty DataTemplateSelectorProperty = RegisterProperty(
		propertyGrid => propertyGrid.DataTemplateSelector, new FrameworkPropertyMetadata()
		{
			DefaultValue = null,
			AffectsRender = true,
			AffectsMeasure = true,
			PropertyChangedCallback = DataTemplateSelectorChanged,
		});

	private static readonly DependencyPropertyKey PropertyItemsPropertyKey = RegisterReadOnlyProperty(
		propertyGrid => propertyGrid.PropertyItems, new FrameworkPropertyMetadata()
		{
			DefaultValue = null,
			AffectsRender = true,
			AffectsMeasure = true,
		});

	public static readonly DependencyProperty PropertyItemsProperty = PropertyItemsPropertyKey.DependencyProperty;

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
			.Select(propertySource => propertySource is not null
				? PropertyGridItem.Scan(propertySource)
				: Enumerable.Empty<PropertyInfo>())
			.Select(propertyInfo => propertyInfo
				.Where(FilterPropertyInfo)
				.Select(PropertyGridItem.Create)
				.AsObservableChangeSet(item => item.PropertyName))
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