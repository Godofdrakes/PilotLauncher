using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.Common;

public class WorkflowBranch : ReactiveObject, IWorkflowNode
{
	public string Label => "sequence";

	public string Description => "run in sequence";

	public IEnumerable<IWorkflowNode> Children => _children;

	private readonly ReadOnlyObservableCollection<IWorkflowNode> _children;
	private readonly SourceCache<IWorkflowNode, int> _sourceCache;

	public WorkflowBranch()
	{
		_sourceCache = new SourceCache<IWorkflowNode, int>(node => node.GetHashCode());
		_sourceCache.Connect()
			.Bind(out _children)
			.Subscribe();
	}

	public WorkflowBranch Add(params IWorkflowNode[] nodes)
	{
		_sourceCache.AddOrUpdate(nodes);
		return this;
	}

	public WorkflowBranch Sequence(params IWorkflowNode[] nodes)
	{
		return Add(new WorkflowBranch().Add(nodes));
	}

	public void Remove(IWorkflowNode node) => _sourceCache.Remove(node);
}