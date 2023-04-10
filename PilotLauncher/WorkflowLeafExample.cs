using System.Reactive;
using System.Reactive.Linq;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher;

public sealed class WorkflowLeafExample : WorkflowLeaf
{
	public override string Label => _label.Value;

	public override ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
	public override ReactiveCommand<Unit, Unit> CancelCommand { get; }

	private readonly ObservableAsPropertyHelper<string> _label;

	public int DelaySeconds
	{
		get => _delaySeconds;
		set => this.RaiseAndSetIfChanged(ref _delaySeconds, value);
	}

	private int _delaySeconds = 5;

	public WorkflowLeafExample()
	{
		CancelCommand = ReactiveCommand.Create(() => { });
		ExecuteCommand = ReactiveCommand.CreateFromObservable(() =>
			Observable.Timer(TimeSpan.FromSeconds(5))
				.TakeUntil(CancelCommand)
				.Select(_ => Unit.Default));
		
		_label = this.WhenAnyValue(x => x.DelaySeconds)
			.Select(seconds => $"wait {seconds} seconds")
			.ToProperty(this, x => x.Label);
	}
}