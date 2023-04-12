using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher;

public sealed class WorkflowLeafExample : WorkflowLeaf
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

	private string _message = string.Empty;

	[ReactivePropertyInfo]
	public string FullMessage => _fullMessage.Value;

	private ObservableAsPropertyHelper<string> _fullMessage;

	[ReactivePropertyInfo]
	public int Delay
	{
		get => _delay;
		set => this.RaiseAndSetIfChanged(ref _delay, value);
	}

	private int _delay = 5;

	public ILogger<WorkflowLeafExample>? Logger { get; set; }

	public WorkflowLeafExample()
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

		_fullMessage = this.WhenAnyValue(x => x.Delay, x => x.Message)
			.Select(tuple =>
				$"I've waited {tuple.Item1} seconds to tell you: {(string.IsNullOrEmpty(tuple.Item2) ? "nothing" : tuple.Item2)}")
			.ToProperty(this, example => example.FullMessage);
	}
}