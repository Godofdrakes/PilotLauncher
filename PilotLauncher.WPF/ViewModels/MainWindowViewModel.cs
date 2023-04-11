using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.Logging;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class MainWindowViewModel : WindowViewModel, IMainWindowViewModel
{
	public MainWindowInteractions Interactions { get; } = new();

	public ReactiveCommand<IWorkflowNode,Unit> ExecuteCommand { get; }

	public WorkflowBranch WorkflowRoot { get; }

	public IObserver<string> ConsoleOutputObserver { get; }

	public ReadOnlyObservableCollection<string> ConsoleOutput => _consoleOutput;

	private readonly ReadOnlyObservableCollection<string> _consoleOutput;

	public MainWindowViewModel(ILoggerFactory loggerFactory)
	{
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
					DelaySeconds = 1,
					Logger = loggerFactory.CreateLogger<WorkflowLeafExample>(),
				},
				new WorkflowLeafExample
				{
					DelaySeconds = 2,
					Logger = loggerFactory.CreateLogger<WorkflowLeafExample>(),
				}
			},
			new WorkflowLeafExample
			{
				DelaySeconds = 3,
				Logger = loggerFactory.CreateLogger<WorkflowLeafExample>(),
			}
		};

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

	private static IEnumerable<WorkflowLeaf> GetWorkflowQueue(IWorkflowNode root)
	{
		return root.Flatten(node => node.Children).OfType<WorkflowLeaf>();
	}
}