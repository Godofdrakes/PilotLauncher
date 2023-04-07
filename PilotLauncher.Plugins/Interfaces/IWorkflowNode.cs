using System.Reactive;
using ReactiveUI;

namespace PilotLauncher.Plugins.Interfaces;

public interface IWorkflowNode
{
	string Label { get; }

	Exception? LastException { get; }

	ReactiveCommand<Unit,Unit> ExecuteCommand { get; }
	ReactiveCommand<Unit,Unit> CancelCommand { get; }

	bool HasChildren { get; }

	IEnumerable<IWorkflowNode> Children { get; }
}