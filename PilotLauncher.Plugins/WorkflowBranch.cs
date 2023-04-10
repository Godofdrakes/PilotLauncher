using System.Collections.ObjectModel;
using System.Reactive;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public sealed class WorkflowBranch : ReactiveObject, IWorkflowNode
{
	public string Label { get; } = "sequence";

	public ReactiveCommand<IWorkflowNode, Unit> AddCommand { get; }
	public ReactiveCommand<IWorkflowNode, Unit> RemoveCommand { get; }

	public IEnumerable<IWorkflowNode> Children => _children;

	private readonly ReadOnlyObservableCollection<IWorkflowNode> _children;

	public WorkflowBranch()
	{
		var source = new SourceCache<IWorkflowNode, int>(node => node.GetHashCode());
		source.Connect()
			.Bind(out _children)
			.Subscribe();

		AddCommand = ReactiveCommand.Create((IWorkflowNode node) => source.AddOrUpdate(node));
		RemoveCommand = ReactiveCommand.Create((IWorkflowNode node) => source.Remove(node));
	}
}