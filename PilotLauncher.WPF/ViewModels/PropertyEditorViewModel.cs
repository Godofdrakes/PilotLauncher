using PilotLauncher.Common;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class PropertyEditorViewModel : ReactiveObject
{
	private IWorkflowNode? _selectedNode;

	public IWorkflowNode? SelectedNode
	{
		get => _selectedNode;
		set => this.RaiseAndSetIfChanged(ref _selectedNode, value);
	}
}