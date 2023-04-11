using System.Diagnostics;
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
	public int DelaySeconds
	{
		get => _delaySeconds;
		set => this.RaiseAndSetIfChanged(ref _delaySeconds, value);
	}

	private int _delaySeconds = 5;

	public ILogger<WorkflowLeafExample>? Logger { get; set; }

	public WorkflowLeafExample()
	{
		CancelCommand = ReactiveCommand.Create(() => { });
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() =>
			Observable.Interval(TimeSpan.FromSeconds(1))
				.Take(DelaySeconds)
				.TakeUntil(CancelCommand)
				.Do(i => Logger?.LogInformation("{seconds}", i + 1))
				.Select(_ => Unit.Default));
		
		_label = this.WhenAnyValue(x => x.DelaySeconds)
			.Select(seconds => $"wait {seconds} seconds")
			.ToProperty(this, x => x.Label);
	}
}