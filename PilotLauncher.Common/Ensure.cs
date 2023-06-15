using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace PilotLauncher.Common;

public static class Ensure
{
	[StackTraceHidden]
	public static void Argument<T>(
		T param,
		Expression<Func<T, bool>> assertion,
		string? message = default,
		[CallerArgumentExpression("param")] string paramName = "")
	{
		if (assertion.Compile().Invoke(param) == false)
		{
			ThrowArgumentException(message, assertion, paramName);
		}
	}

	[StackTraceHidden]
	public static void That(Expression<Func<bool>> expression)
	{
		var condition = expression.Compile();

		if (condition() == false)
		{
			ThrowEnsureFailed(expression);
		}
	}

	[DoesNotReturn, StackTraceHidden]
	private static void ThrowArgumentException(string? message, LambdaExpression assertion, string paramName)
	{
		throw new ArgumentException(message ?? $"Ensure failed: {assertion.Body}", paramName);
	}

	[DoesNotReturn]
	[StackTraceHidden]
	private static void ThrowEnsureFailed(Expression<Func<bool>> expression) =>
		throw new InvalidOperationException($"Ensure failed: {expression.Body}");
}