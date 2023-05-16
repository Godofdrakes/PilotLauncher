using System;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PilotLauncher.WPF.ViewModels;
using PilotLauncher.WPF.Views;
using ReactiveUI;

namespace PilotLauncher.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IHostEnvironment _hostEnvironment;
	private readonly ILoggerFactory _loggerFactory;

	public App(
		IServiceProvider serviceProvider,
		IHostEnvironment hostEnvironment,
		ILoggerFactory loggerFactory)
	{
		_serviceProvider = serviceProvider;
		_hostEnvironment = hostEnvironment;
		_loggerFactory = loggerFactory;

		InitializeComponent();
	}

	private void App_OnStartup(object sender, StartupEventArgs e)
	{
		var mainWindowView = _serviceProvider.GetRequiredService<MainWindow>();

		mainWindowView.ViewModel = new MainWindowViewModel(_loggerFactory)
		{
			Title = _hostEnvironment.ApplicationName,
		};

		MainWindow = mainWindowView;
		MainWindow.Show();
	}
}