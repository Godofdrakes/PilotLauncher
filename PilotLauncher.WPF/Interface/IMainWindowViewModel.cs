using System.Collections.ObjectModel;
using System.Reactive;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher.WPF;

public interface IMainWindowViewModel
{
	ReactiveCommand<IWorkflowNode,Unit> ExecuteCommand { get; }
	WorkflowBranch WorkflowRoot { get; }
}