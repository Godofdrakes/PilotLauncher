using System.Collections.ObjectModel;
using System.Reactive;
using PilotLauncher.Common;
using PropertyDetails.Interfaces;
using ReactiveUI;

namespace PilotLauncher.WPF.Design;

public class MainWindowViewModel : IMainWindowViewModel
{
	public ReactiveCommand<IWorkflowNode, Unit> ExecuteCommand { get; }

	public ReadOnlyObservableCollection<IPropertyDetails> WorkflowProperties { get; }

	public WorkflowBranch WorkflowRoot { get; } = new WorkflowBranch()
		.Sequence(
			new WorkflowStepExample { Delay = 1 },
			new WorkflowStepExample { Delay = 2 })
		.Add(new WorkflowStepExample { Delay = 3 });
}