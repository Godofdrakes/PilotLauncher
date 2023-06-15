using System;
using System.Linq.Expressions;
using System.Windows;
using PilotLauncher.Common;
using ReactiveUI;

namespace PilotLauncher.WPF.Common;

public static class DependencyObjectEx
{
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

	private static TReturn RegisterPropertyCommon<TObject, TProperty, TReturn>(
		Func<string, Type, Type, PropertyMetadata?, ValidateValueCallback?, TReturn> registerFunc,
		Expression<Func<TObject, TProperty>> propertyExpression,
		PropertyMetadata? propertyMetadata = default,
		ValidateValueCallback? validateValueCallback = default)
	{
		ArgumentNullException.ThrowIfNull(registerFunc);
		ArgumentNullException.ThrowIfNull(propertyExpression);

		var expression = Reflection.Rewrite(propertyExpression.Body);

		var parent = expression.GetParent();

		Ensure.Argument(parent, expr => expr != null,
			"The property expression does not have a valid parent.");
		Ensure.Argument(parent, expr => expr!.NodeType == ExpressionType.Parameter,
			"Property expression must be of the form 'x => x.SomeProperty'");

		var memberInfo = expression.GetMemberInfo();

		Ensure.Argument(memberInfo, expr => expr != null,
			"The property expression does not point towards a valid member.");

		var memberName = memberInfo!.Name;
		if (expression is IndexExpression)
		{
			memberName += "[]";
		}

		return registerFunc.Invoke(
			memberName,
			typeof(TProperty),
			typeof(TObject),
			propertyMetadata,
			validateValueCallback);
	}

	public static DependencyProperty RegisterProperty<TObject, TProperty>(
		Expression<Func<TObject, TProperty>> propertyExpression,
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

	public static DependencyPropertyKey RegisterReadOnlyProperty<TObject, TProperty>(
		Expression<Func<TObject, TProperty>> propertyExpression,
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
}