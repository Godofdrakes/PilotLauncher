using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PilotLauncher.WPF.ViewModels;
using PilotLauncher.WPF.Views;
using ReactiveUI;

namespace PilotLauncher.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		private readonly ILogger<App> _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly IHostEnvironment _hostEnvironment;
		
		public App(
			ILogger<App> logger,
			IServiceProvider serviceProvider,
			IHostEnvironment hostEnvironment)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_hostEnvironment = hostEnvironment;

			InitializeComponent();
		}

		private void App_OnStartup(object sender, StartupEventArgs e)
		{
			_logger.TraceFunction();

			var mainWindowView = _serviceProvider.GetRequiredService<MainWindow>();

			mainWindowView.ViewModel = new MainWindowViewModel
			{
				Title = _hostEnvironment.ApplicationName,
			};

			this.MainWindow = mainWindowView;
			this.MainWindow!.Show();
		}
	}
}