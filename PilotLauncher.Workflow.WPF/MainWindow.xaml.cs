using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using PilotLauncher.Examples;
using PilotLauncher.WorkflowLog;
using ReactiveUI;

namespace PilotLauncher.Workflow.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public WorkflowViewModel Workflow { get; }

	public WorkflowNodeFactoryViewModel Factory { get; }

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

		InitializeComponent();

		var log = serviceProvider.GetRequiredService<IWorkflowLog>();
		log.Connect()
			.Filter(entry => entry.Source.Contains(nameof(PilotLauncher)))
			.QueryWhenChanged()
			.Select(lines => string
				.Join(Environment.NewLine, lines.Select(line => line.Message)))
			.ObserveOn(Dispatcher)
			.BindTo(TextBox, textBox => textBox.Text);
	}
}