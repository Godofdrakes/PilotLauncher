using System;
using System.Windows.Markup;
using PilotLauncher.Plugins;

namespace PilotLauncher.WPF;

[MarkupExtensionReturnType(typeof(Type))]
public class ReactivePropertyInfoType<T> : MarkupExtension
{
	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		return typeof(ReactivePropertyInfo<T>);
	}
}