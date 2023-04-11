using System.Reactive;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher.WPF;

public enum ShowConsoleOutputMode
{
	Show,
	Toggle,
}

public sealed class MainWindowInteractions
{
	public Interaction<IWorkflowNode, Unit> ShowEditFlyout { get; } = new();
	public Interaction<ShowConsoleOutputMode, Unit> ShowConsoleOutput { get; } = new();
	public Interaction<Unit, Unit> ClearConsoleOutput { get; } = new();

	public ReactiveCommand<IWorkflowNode, Unit> ShowEditFlyoutCommand { get; }
	public ReactiveCommand<Unit, Unit> ShowConsoleOutputCommand { get; }
	public ReactiveCommand<Unit, Unit> ToggleConsoleOutputCommand { get; }
	public ReactiveCommand<Unit, Unit> ClearConsoleOutputCommand { get; }

	public MainWindowInteractions()
	{
		ShowEditFlyoutCommand = ReactiveCommand.CreateFromObservable<IWorkflowNode, Unit>(node =>
			ShowEditFlyout.Handle(node));
		ShowConsoleOutputCommand = ReactiveCommand.CreateFromObservable(() =>
			ShowConsoleOutput.Handle(ShowConsoleOutputMode.Show));
		ToggleConsoleOutputCommand = ReactiveCommand.CreateFromObservable(() =>
			ShowConsoleOutput.Handle(ShowConsoleOutputMode.Toggle));
		ClearConsoleOutputCommand = ReactiveCommand.CreateFromObservable(() =>
			ClearConsoleOutput.Handle(Unit.Default));
	}
}