using System.Collections.ObjectModel;
using PilotLauncher.Plugins;

namespace PilotLauncher.WPF;

public interface IEditWorkflowViewModel
{
	public IWorkflowNode? WorkflowNode { get; set; }

	public ReadOnlyObservableCollection<ReactivePropertyInfo> PropertyInfo { get; }
}