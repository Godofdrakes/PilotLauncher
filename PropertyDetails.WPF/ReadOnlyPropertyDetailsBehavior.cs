using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using PropertyDetails.Interfaces;

namespace PropertyDetails.WPF;

public class ReadOnlyPropertyDetailsBehavior : Behavior<DataGrid>
{
	protected override void OnAttached() => AssociatedObject.BeginningEdit += OnBeginningEdit;
	protected override void OnDetaching() => AssociatedObject.BeginningEdit -= OnBeginningEdit;

	private static void OnBeginningEdit(object? sender, DataGridBeginningEditEventArgs args)
	{
		if (args.Row.Item is IPropertyDetails { CanWrite: false })
		{
			args.Cancel = true;
		}
	}
}