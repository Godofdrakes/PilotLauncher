using System.Reactive;
using PilotLauncher.Plugins.Interfaces;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public sealed class WorkflowBranch : ReactiveObject, IWorkflowNode
{
	public string Label { get; }
	public Exception? LastException { get; }
	public ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
	public ReactiveCommand<Unit, Unit> CancelCommand { get; }
	public bool HasChildren { get; }
	public IEnumerable<IWorkflowNode> Children { get; }

	public bool IsParallel
	{
		get => isParallel;
		set => this.RaiseAndSetIfChanged(ref isParallel, value);
	}

	private bool isParallel = false;
}