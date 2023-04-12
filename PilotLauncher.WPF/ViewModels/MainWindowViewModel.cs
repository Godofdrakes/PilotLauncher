using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using DynamicData;
using Microsoft.Extensions.Logging;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class MainWindowViewModel : WindowViewModel, IMainWindowViewModel
{
	public EditWorkflowViewModel EditWorkflowViewModel { get; } = new();

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

		WorkflowRoot = new WorkflowBranch
		{
			new WorkflowBranch
			{
				new WorkflowLeafExample
				{
					Delay = 1,
					Logger = loggerFactory.CreateLogger<WorkflowLeafExample>(),
				},
				new WorkflowLeafExample
				{
					Delay = 2,
					Logger = loggerFactory.CreateLogger<WorkflowLeafExample>(),
				},
			},
			new WorkflowLeafExample
			{
				Delay = 3,
				Logger = loggerFactory.CreateLogger<WorkflowLeafExample>(),
			},
		};
	}

	private static IEnumerable<WorkflowLeaf> GetWorkflowQueue(IWorkflowNode root)
	{
		return root.Flatten(node => node.Children).OfType<WorkflowLeaf>();
	}
}