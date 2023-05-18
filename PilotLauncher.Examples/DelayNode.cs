using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PilotLauncher.Workflow;
using ReactiveUI;

namespace PilotLauncher.Examples;

public sealed class DelayNode : WorkflowNodeViewModel
{
	public override ReactiveCommand<Unit, Unit> ExecuteCommand { get; }

	public int Seconds
	{
		get => _seconds;
		set => this.RaiseAndSetIfChanged(ref _seconds, value);
	}

	private int _seconds = 1;

	public DelayNode(ILogger<DelayNode> logger)
	{
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() => Observable
			.Timer(TimeSpan.FromSeconds(Seconds))
			.Do(seconds => logger.LogDebug("Waited {Seconds} seconds", seconds))
			.Select(_ => Unit.Default));
	}
}