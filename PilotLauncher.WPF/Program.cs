using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PilotLauncher.Common;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace PilotLauncher.WPF;

public static class Program
{
	[STAThread]
	public static Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.ConfigureLogging()
			.ConfigureSingleInstance()
			.ConfigureSplat()
			.ConfigureReactiveUIViews()
			.ConfigureWpf<App>();

		return host
			.Build()
			.MapSplatLocator()
			.RunAsync();
	}

	private static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
	{
		return hostBuilder.ConfigureLogging(builder =>
		{
			builder.AddConsole();
			builder.AddDebug();
		});
	}

	private static IHostBuilder ConfigureSingleInstance(this IHostBuilder hostBuilder)
	{
		return hostBuilder.ConfigureSingleInstance(builder =>
		{
			const string singleInstanceMutexId = "{D57B9A73-D6B0-4EA9-ABEA-50A483B159FC}";

			builder.MutexId = singleInstanceMutexId;
			builder.WhenNotFirstInstance = (environment, logger) =>
			{
				logger.LogWarning("An instance of the application already exists");
			};
		});
	}

	private static IHostBuilder ConfigureReactiveUIViews(this IHostBuilder hostBuilder)
	{
		return hostBuilder.ConfigureServices(builder =>
		{
			builder.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
		});
	}

	private static IHostBuilder ConfigureWpf<T>(this IHostBuilder hostBuilder)
		where T : Application
	{
		return hostBuilder
			.ConfigureWpf(builder => builder.UseApplication<T>())
			.UseWpfLifetime();
	}

	private static IHostBuilder ConfigureSplat(this IHostBuilder hostBuilder)
	{
		return hostBuilder.ConfigureServices(serviceCollection =>
		{
			serviceCollection.UseMicrosoftDependencyResolver();
			var resolver = Locator.CurrentMutable;
			resolver.InitializeSplat();
			resolver.InitializeReactiveUI();
		});
	}

	private static IHost MapSplatLocator(this IHost host)
	{
		var c = host.Services;
		c.UseMicrosoftDependencyResolver();
		return host;
	}
}