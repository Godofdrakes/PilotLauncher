using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PilotLauncher.Workflow;
using ReactiveUI;

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
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() => Observable
			.Return(Message)
			.Do(message => logger.LogInformation("Message: {Message}", message))
			.Select(_ => Unit.Default));
	}
}