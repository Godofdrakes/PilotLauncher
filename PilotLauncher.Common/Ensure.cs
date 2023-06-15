using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace PilotLauncher.Common;

public static class Ensure
{
	[StackTraceHidden]
	public static void True(Expression<Func<bool>> expression) =>
		Ensure.That(expression, true);

	[StackTraceHidden]
	public static void True(Expression<Func<bool>> expression, string paramName) =>
		Ensure.That(expression, true, paramName);

	[StackTraceHidden]
	public static void False(Expression<Func<bool>> expression) =>
		Ensure.That(expression, false);

	[StackTraceHidden]
	public static void False(Expression<Func<bool>> expression, string paramName) =>
		Ensure.That(expression, false, paramName);

	[StackTraceHidden]
	private static void That(Expression<Func<bool>> expression, bool expected, string paramName)
	{
		var condition = expression.Compile();

		if (condition() != expected)
		{
			ThrowEnsureFailed(expression, paramName);
		}
	}

	[StackTraceHidden]
	private static void That(Expression<Func<bool>> expression, bool expected)
	{
		var condition = expression.Compile();

		if (condition() != expected)
		{
			ThrowEnsureFailed(expression);
		}
	}

	[DoesNotReturn]
	[StackTraceHidden]
	private static void ThrowEnsureFailed(Expression<Func<bool>> expression) =>
		throw new InvalidOperationException($"Ensure condition failed: {expression.Body}");

	[DoesNotReturn]
	[StackTraceHidden]
	private static void ThrowEnsureFailed(Expression<Func<bool>> expression, string paramName) =>
		throw new ArgumentException($"Ensure condition failed: {expression.Body}", paramName);
}