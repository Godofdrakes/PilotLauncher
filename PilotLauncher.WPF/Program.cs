using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Plugins;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace PilotLauncher.WPF;

public static class Program
{
	private static DirectoryInfo? GetSolutionDir(IHostEnvironment hostEnvironment)
	{
		var dir = new DirectoryInfo(hostEnvironment.ContentRootPath);

		while (dir != null && !dir.EnumerateFiles().Any(file => file.Extension.Equals(".sln")))
		{
			dir = dir.Parent;
		}

		return dir;
	}

	private static IEnumerable<DirectoryInfo> GetPluginDirs(DirectoryInfo solutionDir)
	{
		return solutionDir.GetDirectories("Plugins/*/bin/Debug/net6.0", SearchOption.AllDirectories);
	}

	[STAThread]
	public static Task Main(string[] args)
	{
		var tempHost = Host.CreateDefaultBuilder(args)
			.Build();

		var hostEnvironment = tempHost.Services.GetRequiredService<IHostEnvironment>();

		var host = Host.CreateDefaultBuilder(args)
			.ConfigureSingleInstance()
			.ConfigureSplatForMicrosoftDependencyResolver()
			.ConfigureReactiveUIViews()
			.ConfigurePlugins()
			.ConfigureWpf<App>();

		if (hostEnvironment.IsDevelopment())
		{
			host.ConfigurePlugins(builder =>
			{
				
			});
		}

		return host
			.Build()
			.MapSplatLocator()
			.RunAsync();
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

	private static IHostBuilder ConfigurePlugins(this IHostBuilder hostBuilder)
	{
		return hostBuilder.ConfigurePlugins(builder =>
		{
			builder.AssemblyScanFunc = PluginScanner.ByNamingConvention;
			builder.UseContentRoot = true;
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

	private static IHostBuilder ConfigureSplatForMicrosoftDependencyResolver(this IHostBuilder hostBuilder)
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