using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

// ReSharper disable UnusedMethodReturnValue.Local

namespace PilotLauncher.WorkflowLogging;

public static class WorkflowLogHostExtensions
{
	public static ILoggingBuilder AddWorkflowLog(this ILoggingBuilder builder, Action<WorkflowLogOptions> configure)
	{
		builder.AddWorkflowLog();
		builder.Services.Configure(configure);

		return builder;
	}

	private static ILoggingBuilder AddWorkflowLog(this ILoggingBuilder builder)
	{
		builder.AddConfiguration();

		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, WorkflowLoggerProvider>());
		builder.Services.TryAddSingleton<WorkflowLog>();
		builder.Services.TryAddTransient<IWorkflowLog>(provider => provider.GetRequiredService<WorkflowLog>());
		LoggerProviderOptions.RegisterProviderOptions<WorkflowLogOptions, WorkflowLoggerProvider>(builder.Services);

		return builder;
	}
}