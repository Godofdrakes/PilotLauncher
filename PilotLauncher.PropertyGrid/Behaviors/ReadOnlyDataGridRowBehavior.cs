using System.Windows.Controls;
using System.Windows.Interactivity;

namespace PilotLauncher.PropertyGrid;

public class ReadOnlyDataGridRowBehavior : Behavior<DataGrid>
{
	protected override void OnAttached() => AssociatedObject.BeginningEdit += OnBeginningEdit;
	protected override void OnDetaching() => AssociatedObject.BeginningEdit -= OnBeginningEdit;

	private static void OnBeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
	{
		if (e.Row.Item is PropertyGridItem { IsReadOnly: true })
		{
			e.Cancel = true;
		}
	}
}