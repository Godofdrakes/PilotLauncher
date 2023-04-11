using System.Collections;
using System.Reactive;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public abstract class WorkflowLeaf : ReactivePrototypeObject, IWorkflowNode
{
	public abstract string Label { get; }
	
	public abstract ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
	public abstract ReactiveCommand<Unit, Unit> CancelCommand { get; }

	public IEnumerable<IWorkflowNode> Children => Enumerable.Empty<IWorkflowNode>();

	public IEnumerator<IWorkflowNode> GetEnumerator() => Children.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
}