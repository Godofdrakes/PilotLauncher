using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PilotLauncher.Common;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace PilotLauncher.WPF;

public static class Program
{
	private const int ATTACH_PARENT_PROCESS = -1;

	// Rider seem to receive console logging properly with WPF apps. This fixes that.
	[DllImport("kernel32.dll")] private static extern bool AttachConsole(int dwProcessId);

	[STAThread]
	public static Task Main(string[] args)
	{
#if DEBUG
		AttachConsole(ATTACH_PARENT_PROCESS);
#endif

		var host = Host.CreateDefaultBuilder(args)
#if DEBUG
			.UseEnvironment(Environments.Development)
#endif
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
			builder.AddSimpleConsole(options =>
			{
				options.SingleLine = true;
				options.IncludeScopes = true;
				options.TimestampFormat = "HH:mm:ss:";
			});
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