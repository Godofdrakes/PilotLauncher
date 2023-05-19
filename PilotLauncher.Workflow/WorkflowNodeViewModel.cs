using System.Reactive;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;

namespace PilotLauncher.Workflow;

public abstract class WorkflowNodeViewModel : ReactiveValidationObject
{
	public Guid Id { get; } = Guid.NewGuid();

	public abstract ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
}