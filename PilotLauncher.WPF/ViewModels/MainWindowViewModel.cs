﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using PilotLauncher.Plugins;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class MainWindowViewModel : WindowViewModel
{
	public MainWindowInteractions Interactions { get; } = new();

	public ReactiveCommand<IWorkflowNode,Unit> ExecuteCommand { get; }

	public WorkflowBranch WorkflowRoot { get; } = new();

	public IObserver<string> ConsoleOutputObserver { get; }

	public ReadOnlyObservableCollection<string> ConsoleOutput => _consoleOutput;

	private readonly ReadOnlyObservableCollection<string> _consoleOutput;

	public MainWindowViewModel()
	{
		ExecuteCommand = ReactiveCommand.CreateFromObservable<IWorkflowNode, Unit>(node =>
		{
			return GetWorkflowQueue(node)
				.ToObservable()
				// Commands will be executed as they are subscribed to
				.Select(leaf => leaf.ExecuteCommand.Execute())
				// Concat to subscribe in squence
				.Concat();
		});

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

	private static Queue<WorkflowLeaf> GetWorkflowQueue(IWorkflowNode root)
	{
		var leaves = new Queue<WorkflowLeaf>();
		var queue = new Queue<IWorkflowNode>();

		queue.Enqueue(root);

		while (queue.TryDequeue(out var node))
		{
			foreach (var child in node.Children)
			{
				if (child is WorkflowLeaf leaf)
				{
					leaves.Enqueue(leaf);
				}
				else
				{
					queue.Enqueue(child);
				}
			}
		}

		return leaves;
	}
}