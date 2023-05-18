using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace PilotLauncher.Workflow.WPF;

internal class NullServiceProvider : IServiceProvider
{
	private readonly NullLoggerFactory _loggerFactory = new();

	public object? GetService(Type serviceType)
	{
		if (serviceType.IsAssignableTo(typeof(ILogger)))
		{
			var sourceType = serviceType.GetGenericArguments().First();
			var loggerType = typeof(Logger<>).MakeGenericType(sourceType);
			return Activator.CreateInstance(loggerType, _loggerFactory);
		}

		throw new NotImplementedException();
	}
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public WorkflowViewModel Workflow { get; } = new();

	public WorkflowNodeFactoryViewModel Factory { get; }

	public MainWindow()
	{
		Factory = new WorkflowNodeFactoryViewModel(new NullServiceProvider());

		Factory.CreateNodeCommand
			.ObserveOn(Dispatcher)
			.Subscribe(node => Workflow.Add(node));

		InitializeComponent();
	}
}