using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using PilotLauncher.Examples;
using PilotLauncher.Workflow;
using PilotLauncher.WorkflowLogging;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class MainWindowViewModel : WindowViewModel
{
	public MainWindowInteractions Interactions { get; } = new();

	public WorkflowViewModel Workflow { get; }

	public IEnumerable<WorkflowLogEntry> LogOutput => _logOutput;

	private readonly ReadOnlyObservableCollection<WorkflowLogEntry> _logOutput;

	public MainWindowViewModel(IServiceProvider serviceProvider)
	{
		Workflow = serviceProvider.GetRequiredService<WorkflowViewModel>();
		Workflow.Add(ActivatorUtilities.CreateInstance<DelayNode>(serviceProvider));
		Workflow.Add(ActivatorUtilities.CreateInstance<EchoNode>(serviceProvider));

		serviceProvider.GetRequiredService<IWorkflowLog>()
			.Connect()
			.Filter(entry => entry.Source.Contains(nameof(PilotLauncher)))
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _logOutput)
			.Subscribe();
	}
}