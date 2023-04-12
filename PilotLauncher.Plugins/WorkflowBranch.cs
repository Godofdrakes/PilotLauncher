using System.Collections;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public sealed class WorkflowBranch : ReactiveObject, IWorkflowNode
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

	public void Add(IWorkflowNode node) => _sourceCache.AddOrUpdate(node);
	public void Remove(IWorkflowNode node) => _sourceCache.Remove(node);

	public IEnumerator<IWorkflowNode> GetEnumerator() => _children.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
}