using System.Reactive;
using PilotLauncher.Common;
using ReactiveUI;

namespace PilotLauncher.WPF;

public interface IMainWindowViewModel
{
	ReactiveCommand<IWorkflowNode,Unit> ExecuteCommand { get; }
	WorkflowBranch WorkflowRoot { get; }
}