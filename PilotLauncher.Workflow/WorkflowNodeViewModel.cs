using System.Reactive;
using ReactiveUI;

namespace PilotLauncher.Workflow;

public abstract class WorkflowNodeViewModel : ReactiveObject
{
	public Guid Id { get; } = Guid.NewGuid();

	public abstract ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
}