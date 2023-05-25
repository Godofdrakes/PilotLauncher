using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PilotLauncher.Workflow;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

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
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() =>
		{
			logger.LogInformation("Invoking observable factory");

			var seconds = Seconds;

			return Observable
				.Timer(TimeSpan.FromSeconds(Seconds))
				.Do(_ => logger.LogInformation(
					"Waited {Seconds} second(s)", seconds))
				.Select(_ => Unit.Default);
		});

		this.ValidationRule(node => node.Seconds,
			seconds => seconds > 0,
			"You must specify a positive value");
	}
}