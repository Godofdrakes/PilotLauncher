using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PilotLauncher.Workflow;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PilotLauncher.Examples;

public sealed class EchoNode : WorkflowNodeViewModel
{
	public override ReactiveCommand<Unit, Unit> ExecuteCommand { get; }

	public string Message
	{
		get => _message;
		set => this.RaiseAndSetIfChanged(ref _message, value);
	}

	private string _message = "echo";

	public EchoNode(ILogger<EchoNode> logger)
	{
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() =>
		{
			logger.LogInformation(
				"{Node}: Invoking observable factory",
				nameof(EchoNode));

			return Observable
				.Return(Message)
				.Do(message => logger.LogInformation(
					"{Node}: {Message}",
					nameof(EchoNode), message))
				.Select(_ => Unit.Default);
		});

		this.ValidationRule(node => node.Message,
			message => !string.IsNullOrEmpty(message),
			"You must specify a message");
	}
}