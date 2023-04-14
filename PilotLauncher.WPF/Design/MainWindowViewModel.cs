using System.Collections.ObjectModel;
using System.Reactive;
using PilotLauncher.Plugins;
using PropertyDetails.Interfaces;
using ReactiveUI;

namespace PilotLauncher.WPF.Design;

public class MainWindowViewModel : IMainWindowViewModel
{
	public ReactiveCommand<IWorkflowNode, Unit> ExecuteCommand { get; }

	public ReadOnlyObservableCollection<IPropertyDetails> WorkflowProperties { get; }

	public WorkflowBranch WorkflowRoot { get; } = new()
	{
		new WorkflowBranch
		{
			new WorkflowStepExample
			{
				Delay = 1,
			},
			new WorkflowStepExample
			{
				Delay = 2,
			},
		},
		new WorkflowStepExample
		{
			Delay = 3,
		},
	};
}