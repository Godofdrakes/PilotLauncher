using System.Collections.ObjectModel;
using PilotLauncher.Plugins;
using PropertyInspector.Interfaces;

namespace PilotLauncher.WPF;

public interface IEditWorkflowViewModel
{
	public IWorkflowNode? WorkflowNode { get; set; }

	public ReadOnlyObservableCollection<IPropertyInspector> PropertyInfo { get; }
}