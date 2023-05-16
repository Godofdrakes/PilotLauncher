using PilotLauncher.PropertyGrid;
using ReactiveUI;

namespace PilotLauncher.WPF.Views;

public partial class PropertyEditorView
{
	public PropertyEditorView()
	{
		InitializeComponent();

		this.OneWayBind(ViewModel,
			viewModel => viewModel.SelectedNode,
			view => view.PropertyGridView.PropertySource);
	}

	private void PropertyGridView_OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		var propertyType = e.PropertyInfo.PropertyType;
		var declaringType = e.PropertyInfo.DeclaringType;

		if (declaringType?.IsAssignableFrom(typeof(ReactiveObject)) is true)
		{
			e.Cancel = true;
		}

		if (propertyType.IsAssignableTo(typeof(IReactiveCommand)))
		{
			e.Cancel = true;
		}
	}
}