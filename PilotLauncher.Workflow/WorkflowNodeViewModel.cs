using System.Reactive;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;

namespace PilotLauncher.Workflow;

public abstract class WorkflowNodeViewModel : ReactiveValidationObject
{
	public Guid Id { get; } = Guid.NewGuid();

	public string Label => GetType().Name;

	public ReactiveCommand<Unit, Unit> ExecuteCommand { get; }

	protected ILogger Logger { get; }

	protected WorkflowNodeViewModel(ILogger logger)
	{
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() =>
		{
			logger.LogDebug("Invoking observable factory");
			return CreateExecutionObservable();
		}, ValidationContext.Valid);

		Logger = logger;
	}

	protected abstract IObservable<Unit> CreateExecutionObservable();
}