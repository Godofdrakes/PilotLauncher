using System.Runtime.InteropServices;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PilotLauncher.WPF.Common;

public static class WpfHostBuilderEx
{
	private const int ATTACH_PARENT_PROCESS = -1;

	[DllImport("kernel32.dll")] private static extern bool AttachConsole(int dwProcessId);

	public static void FixConsoleLogging()
	{
#if DEBUG
		AttachConsole(ATTACH_PARENT_PROCESS);
#endif
	}

	// Rider doesn't seem to receive console logging properly with WPF apps. This fixes that.
	public static IHostBuilder FixConsoleLogging(this IHostBuilder hostBuilder)
	{
		FixConsoleLogging();
		return hostBuilder;
	}

	public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
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

	public static IHostBuilder ConfigureWpf<T>(this IHostBuilder hostBuilder)
		where T : Application => hostBuilder
		.ConfigureWpf(builder => builder.UseApplication<T>())
		.ConfigureServices(collection => collection.AddTransient<IWpfService, WpfApplicationInitService>())
		.UseWpfLifetime();
}