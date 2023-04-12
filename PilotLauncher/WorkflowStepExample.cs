using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher;

public sealed class WorkflowStepExample : WorkflowStep
{
	public override string Label => _label.Value;

	public override ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
	public override ReactiveCommand<Unit, Unit> CancelCommand { get; }

	private readonly ObservableAsPropertyHelper<string> _label;

	[ReactivePropertyInfo]
	public string Message
	{
		get => _message;
		set => this.RaiseAndSetIfChanged(ref _message, value);
	}

	private string _message = "Hello World!";

	[ReactivePropertyInfo]
	public int Delay
	{
		get => _delay;
		set => this.RaiseAndSetIfChanged(ref _delay, value);
	}

	private int _delay = 5;

	public ILogger<WorkflowStepExample>? Logger { get; set; }

	public WorkflowStepExample()
	{
		CancelCommand = ReactiveCommand.Create(() => { });
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() =>
			Observable.Timer(TimeSpan.FromSeconds(Delay))
				.TakeUntil(CancelCommand)
				.Select(_ => Unit.Default));

		ExecuteCommand.Subscribe(_ => Logger?.LogInformation("{Message}", Message));

		_label = this.WhenAnyValue(x => x.Delay)
			.Select(seconds => $"wait {seconds} seconds")
			.ToProperty(this, x => x.Label);

		DescriptionObservable = this.WhenAnyValue(x => x.Delay, x => x.Message)
			.Select(tuple =>
			{
				var builder = new StringBuilder();
				builder.AppendLine($"Wait {tuple.Item1} seconds");
				builder.Append($"Log \"{tuple.Item2}\"");
				return builder.ToString();
			});
	}
}