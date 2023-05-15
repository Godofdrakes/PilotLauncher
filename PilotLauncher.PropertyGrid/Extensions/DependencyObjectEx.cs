using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid;

public static class DependencyObjectEx
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