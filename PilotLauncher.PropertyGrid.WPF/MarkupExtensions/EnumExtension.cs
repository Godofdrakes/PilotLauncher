using System;
using System.Windows.Markup;

namespace PilotLauncher.PropertyGrid.WPF;

public class EnumExtension : MarkupExtension
{
	private readonly Type _type;

	public EnumExtension(Type type)
	{
		ArgumentNullException.ThrowIfNull(type);

		_type = type;
	}

	public override object? ProvideValue(IServiceProvider serviceProvider)
	{
		return Enum.GetValues(_type);
	}
}