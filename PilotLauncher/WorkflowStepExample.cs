using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using PilotLauncher.Plugins;
using PropertyDetails.Attributes;
using ReactiveUI;

namespace PilotLauncher;

public sealed class WorkflowStepExample : WorkflowStep
{
	public override string Label => _label.Value;

	public override ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
	public override ReactiveCommand<Unit, Unit> CancelCommand { get; }

	private readonly ObservableAsPropertyHelper<string> _label;

	[Inspect]
	public string Message
	{
		get => _message;
		set => this.RaiseAndSetIfChanged(ref _message, value);
	}

	private string _message = "Hello World!";

	[Inspect]
	public int Delay
	{
		get => _delay;
		set => this.RaiseAndSetIfChanged(ref _delay, value);
	}

	private int _delay = 5;

	[Inspect]
	public bool ShouldDelay
	{
		get => _shouldDelay;
		set => this.RaiseAndSetIfChanged(ref _shouldDelay, value);
	}

	private bool _shouldDelay = true;

	public ILogger<WorkflowStepExample>? Logger { get; set; }

	public WorkflowStepExample()
	{
		CancelCommand = ReactiveCommand.Create(() => { });
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() =>
		{
			var observable = Observable.Return<long>(0);

			if (ShouldDelay)
			{
				observable = Observable.Timer(TimeSpan.FromSeconds(Delay))
					.TakeUntil(CancelCommand);
			}

			return observable.Select(_ => Unit.Default);
		});

		ExecuteCommand.Subscribe(_ => Logger?.LogInformation("{Message}", Message));

		_label = this.WhenAnyValue(x => x.Delay)
			.Select(seconds => $"wait {seconds} seconds")
			.ToProperty(this, x => x.Label);

		DescriptionObservable = this.Changed
			.Select(args =>
			{
				var self = args.Sender as WorkflowStepExample;

				var builder = new StringBuilder();

				if (ShouldDelay)
				{
					builder.AppendLine($"Wait {self!.Delay} seconds");
				}

				builder.Append($"Log \"{self!.Message}\"");

				return builder.ToString();
			});
	}
}