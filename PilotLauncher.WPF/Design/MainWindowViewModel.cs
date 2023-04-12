using System.Collections.ObjectModel;
using System.Reactive;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher.WPF.Design;

public class MainWindowViewModel : IMainWindowViewModel
{
	public ReactiveCommand<IWorkflowNode, Unit> ExecuteCommand { get; }

	public ReadOnlyObservableCollection<ReactivePropertyInfo> WorkflowProperties { get; }

	public WorkflowBranch WorkflowRoot { get; } = new()
	{
		new WorkflowBranch
		{
			new WorkflowLeafExample
			{
				Delay = 1,
			},
			new WorkflowLeafExample
			{
				Delay = 2,
			},
		},
		new WorkflowLeafExample
		{
			Delay = 3,
		},
	};
}