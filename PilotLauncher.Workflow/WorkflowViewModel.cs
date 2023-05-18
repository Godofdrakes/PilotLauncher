using System.Collections.ObjectModel;
using System.Reactive;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.Workflow;

public enum WorkflowNodePosition
{
	Top,
	Up,
	Down,
	Bottom,
}

public class WorkflowViewModel : ReactiveObject
{
	public IEnumerable<WorkflowNodeViewModel> Nodes => _nodes;

	public ReactiveCommand<Unit, Unit> ExecuteCommand
	{
		get => throw new NotImplementedException();
	}

	private readonly SourceList<WorkflowNodeViewModel> _nodeList;
	private readonly ReadOnlyObservableCollection<WorkflowNodeViewModel> _nodes;

	public WorkflowViewModel()
	{
		_nodeList = new SourceList<WorkflowNodeViewModel>();
		_nodeList.Connect()
			.Bind(out _nodes)
			.Subscribe();
	}

	public void Add(WorkflowNodeViewModel node) => _nodeList.Add(node);

	public void Move(WorkflowNodeViewModel node, WorkflowNodePosition position) =>
		throw new NotImplementedException();

	public void Remove(WorkflowNodeViewModel node) => _nodeList.Remove(node);
}