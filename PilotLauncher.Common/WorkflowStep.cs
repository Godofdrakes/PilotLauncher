using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace PilotLauncher.Common;

public abstract class WorkflowStep : ReactiveObject, IWorkflowNode
{
	public abstract string Label { get; }

	public string Description => _description.Value;

	private readonly ObservableAsPropertyHelper<string> _description;

	protected IObservable<string> DescriptionObservable
	{
		get => _descriptionObservable;
		set => this.RaiseAndSetIfChanged(ref _descriptionObservable, value);
	}

	private IObservable<string> _descriptionObservable;

	public abstract ReactiveCommand<Unit, Unit> ExecuteCommand { get; }
	public abstract ReactiveCommand<Unit, Unit> CancelCommand { get; }
	
	public IEnumerable<IWorkflowNode> Children => Enumerable.Empty<IWorkflowNode>();

	protected WorkflowStep()
	{
		_descriptionObservable = Observable.Never<string>().Prepend(GetType().Name);
		_description = this.WhenAnyValue(step => step.DescriptionObservable)
			.Switch()
			.ToProperty(this, step => step.Description);
	}
}