using System.Reactive;
using PilotLauncher.Plugins.Interfaces;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public abstract class WorkflowLeafBase : ReactiveObject, IWorkflowNode
{
	public abstract string Label { get; }
	public abstract Exception? LastException { get; }
	public abstract ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
	public abstract ReactiveCommand<Unit, Unit> CancelCommand { get; }

	public bool HasChildren => false;
	public IEnumerable<IWorkflowNode> Children => Enumerable.Empty<IWorkflowNode>();
}