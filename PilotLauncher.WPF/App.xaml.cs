using System;
using System.Reactive.Linq;
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
		private readonly LogObservable _logObservable;

		public App(
			ILogger<App> logger,
			IServiceProvider serviceProvider,
			IHostEnvironment hostEnvironment,
			LogObservable logObservable)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_hostEnvironment = hostEnvironment;
			_logObservable = logObservable;

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

			_logObservable.Output
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(mainWindowView.ViewModel.ConsoleOutputObserver);

			this.MainWindow = mainWindowView;
			this.MainWindow!.Show();
		}
	}
}