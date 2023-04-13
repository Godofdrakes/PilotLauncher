using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using PropertyInspector.Interfaces;

namespace PropertyInspector.WPF;

public class ReadOnlyPropertyInspectorBehavior : Behavior<DataGrid>
{
	protected override void OnAttached() => AssociatedObject.BeginningEdit += OnBeginningEdit;
	protected override void OnDetaching() => AssociatedObject.BeginningEdit -= OnBeginningEdit;

	private static void OnBeginningEdit(object? sender, DataGridBeginningEditEventArgs args)
	{
		if (args.Row.Item is IPropertyInspector { CanWrite: false })
		{
			args.Cancel = true;
		}
	}
}