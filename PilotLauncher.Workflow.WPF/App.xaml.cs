using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace PilotLauncher.Workflow.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
	private readonly IServiceProvider _serviceProvider;

	public App(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	private void App_OnStartup(object sender, StartupEventArgs e)
	{
		MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(_serviceProvider);
		MainWindow.Show();
	}
}