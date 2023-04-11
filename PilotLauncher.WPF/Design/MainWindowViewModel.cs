using System.Reactive;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher.WPF.Design;

public class MainWindowViewModel : IMainWindowViewModel
{
	public ReactiveCommand<IWorkflowNode, Unit> ExecuteCommand { get; } =
		ReactiveCommand.Create<IWorkflowNode>(_ => { });

	public WorkflowBranch WorkflowRoot { get; } = new()
	{
		new WorkflowBranch
		{
			new WorkflowLeafExample
			{
				DelaySeconds = 1,
			},
			new WorkflowLeafExample
			{
				DelaySeconds = 2,
			}
		},
		new WorkflowLeafExample
		{
			DelaySeconds = 3,
		}
	};
}