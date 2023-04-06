using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class MainWindowViewModel : WindowViewModel
{
	public MainWindowInteractions Interactions { get; } = new();

	public IObserver<string> ConsoleOutputObserver { get; }

	public ReadOnlyObservableCollection<string> ConsoleOutput => _consoleOutput;

	private readonly ReadOnlyObservableCollection<string> _consoleOutput;

	public MainWindowViewModel()
	{
		var consoleOutput = new SourceList<string>();
		consoleOutput
			.Connect()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _consoleOutput)
			.Subscribe();

		ConsoleOutputObserver = Observer.Create<string>(
			s => consoleOutput.Add(s),
			e => consoleOutput.Edit(list => list.Add(e.Message)));
	}
}