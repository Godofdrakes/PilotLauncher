using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PilotLauncher.Workflow;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PilotLauncher.Examples;

public sealed class EchoNode : WorkflowNodeViewModel
{
	public string Message
	{
		get => _message;
		set => this.RaiseAndSetIfChanged(ref _message, value);
	}

	private string _message = "echo";

	public EchoNode(ILogger<EchoNode> logger) : base(logger)
	{
		this.ValidationRule(node => node.Message,
			message => !string.IsNullOrEmpty(message),
			"You must specify a message");
	}

	protected override IObservable<Unit> CreateExecutionObservable() => Observable
		.Return(Message)
		.Do(message => Logger.LogInformation(
			"{Message}", message))
		.Select(_ => Unit.Default);
}