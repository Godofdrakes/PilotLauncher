using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Microsoft.Extensions.Logging;
using PilotLauncher.Common;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class MainWindowViewModel : WindowViewModel, IMainWindowViewModel
{
	public PropertyDetailsViewModel PropertyDetailsViewModel { get; } = new();

	public MainWindowInteractions Interactions { get; } = new();

	public ReactiveCommand<IWorkflowNode,Unit> ExecuteCommand { get; }

	public WorkflowBranch WorkflowRoot { get; }

	public IObserver<string> ConsoleOutputObserver => ConsoleOutputSubject.AsObserver();

	private Subject<string> ConsoleOutputSubject { get; } = new();

	public ReadOnlyObservableCollection<string> ConsoleOutput => _consoleOutput;

	private readonly ReadOnlyObservableCollection<string> _consoleOutput;

	public MainWindowViewModel(ILoggerFactory loggerFactory)
	{
		ConsoleOutputSubject.ToObservableChangeSet(limitSizeTo: 10000)
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _consoleOutput)
			.Subscribe();

		ExecuteCommand = ReactiveCommand.CreateFromObservable<IWorkflowNode, Unit>(node =>
		{
			return GetWorkflowQueue(node)
				.ToObservable()
				// Commands will be executed as they are subscribed to
				.Select(leaf => leaf.ExecuteCommand.Execute())
				// Concat to subscribe in sequence
				.Concat();
		});

		WorkflowRoot = new WorkflowBranch()
			.Sequence(
				new WorkflowStepExample { Delay = 1 },
				new WorkflowStepExample { Delay = 2 })
			.Add(new WorkflowStepExample { Delay = 3 });
	}

	private static IEnumerable<WorkflowStep> GetWorkflowQueue(IWorkflowNode root)
	{
		return root.Flatten(node => node.Children).OfType<WorkflowStep>();
	}
}