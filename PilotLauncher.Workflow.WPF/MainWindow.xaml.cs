using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using PilotLauncher.Examples;
using PilotLauncher.WorkflowLog;

namespace PilotLauncher.Workflow.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public WorkflowViewModel Workflow { get; }

	public WorkflowNodeFactoryViewModel Factory { get; }

	public IEnumerable<WorkflowLogEntry> LogOutput => _logOutput;

	private readonly ReadOnlyObservableCollection<WorkflowLogEntry> _logOutput;

	public MainWindow(IServiceProvider serviceProvider)
	{
		Workflow = serviceProvider.GetRequiredService<WorkflowViewModel>();
		Workflow.Add(ActivatorUtilities.CreateInstance<DelayNode>(serviceProvider));
		Workflow.Add(ActivatorUtilities.CreateInstance<EchoNode>(serviceProvider));
		Workflow.Add(ActivatorUtilities.CreateInstance<DelayNode>(serviceProvider));

		Factory = serviceProvider.GetRequiredService<WorkflowNodeFactoryViewModel>();
		Factory.CreateNodeCommand
			.ObserveOn(Dispatcher)
			.Subscribe(node => Workflow.Add(node));

		serviceProvider.GetRequiredService<IWorkflowLog>()
			.Connect()
			.Filter(entry => entry.Source.Contains(nameof(PilotLauncher)))
			.ObserveOn(Dispatcher)
			.Bind(out _logOutput)
			.Subscribe();

		InitializeComponent();
	}
}