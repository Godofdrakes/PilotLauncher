using System.Reactive;
using ReactiveUI;

namespace PilotLauncher.WPF;

public enum ShowConsoleOutputMode
{
	Show,
	Toggle,
}

public sealed class MainWindowInteractions
{
	public Interaction<ShowConsoleOutputMode, Unit> ShowConsoleOutput { get; } = new();
	public Interaction<Unit, Unit> ClearConsoleOutput { get; } = new();

	public ReactiveCommand<Unit, Unit> ShowConsoleOutputCommand { get; }
	public ReactiveCommand<Unit, Unit> ToggleConsoleOutputCommand { get; }
	public ReactiveCommand<Unit, Unit> ClearConsoleOutputCommand { get; }

	public MainWindowInteractions()
	{
		ShowConsoleOutputCommand = ReactiveCommand.CreateFromObservable(() =>
			ShowConsoleOutput.Handle(ShowConsoleOutputMode.Show));
		ToggleConsoleOutputCommand = ReactiveCommand.CreateFromObservable(() =>
			ShowConsoleOutput.Handle(ShowConsoleOutputMode.Toggle));
		ClearConsoleOutputCommand = ReactiveCommand.CreateFromObservable(() =>
			ClearConsoleOutput.Handle(Unit.Default));
	}
}