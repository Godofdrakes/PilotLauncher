using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
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

	public ReactiveCommand<Unit, Unit> ExecuteCommand { get; }

	private readonly SourceList<WorkflowNodeViewModel> _nodeList;
	private readonly ReadOnlyObservableCollection<WorkflowNodeViewModel> _nodes;

	public WorkflowViewModel()
	{
		_nodeList = new SourceList<WorkflowNodeViewModel>();
		_nodeList.Connect()
			.Bind(out _nodes)
			.Subscribe();

		var canExecute = _nodeList.Connect()
			// Collect CanExecute for all nodes
			.Transform(node => node.ExecuteCommand.CanExecute)
			.QueryWhenChanged(observables => observables.CombineLatest())
			.Switch()
			// CanExecute if all true (or node list is empty)
			.Select(list => list.All(canExecute => canExecute));

		ExecuteCommand = ReactiveCommand.CreateFromObservable(() => Nodes
				// Construct ExecuteCommand observables
				.Select(node => node.ExecuteCommand.Execute())
				// Force the above transformation to run for all items immediately.
				// This captures any necessary state at call time preventing changes
				// to the nodes from affecting queued execution.
				.ToArray()
				// Run commands in sequence
				.Concat(),
			canExecute);
	}

	public void Add(WorkflowNodeViewModel node) => _nodeList.Add(node);

	public void Move(WorkflowNodeViewModel node, WorkflowNodePosition position) =>
		throw new NotImplementedException();

	public void Remove(WorkflowNodeViewModel node) => _nodeList.Remove(node);
}