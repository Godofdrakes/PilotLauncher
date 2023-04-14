using System.Collections.ObjectModel;
using PilotLauncher.Plugins;
using PropertyDetails.Interfaces;

namespace PilotLauncher.WPF;

public interface IEditWorkflowViewModel
{
	public IWorkflowNode? WorkflowNode { get; set; }

	public ReadOnlyObservableCollection<IPropertyDetails> PropertyInfo { get; }
}