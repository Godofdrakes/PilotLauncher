using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Data;

namespace PilotLauncher.PropertyGrid;

public static class BindingEx
{
	private class MemberNameVisitor : ExpressionVisitor
	{
		private Stack<MemberInfo> VisitedMembers { get; } = new();

		public override string ToString() =>
			string.Join('.', VisitedMembers.Select(info => info.Name));

		protected override Expression VisitMember(MemberExpression node)
		{
			VisitedMembers.Push(node.Member);

			return base.VisitMember(node);
		}
	}

	public static BindingBase Create(LambdaExpression expression)
	{
		var visitor = new MemberNameVisitor();
		visitor.Visit(expression.Body);
		return new Binding(visitor.ToString());
	}
}