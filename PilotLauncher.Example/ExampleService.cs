using System.Runtime.CompilerServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PilotLauncher.Example;

public class ExampleService : IHostedService
{
	private readonly ILogger<ExampleService> _logger;

	public ExampleService(ILogger<ExampleService> logger, IHostApplicationLifetime lifetime)
	{
		_logger = logger;

		lifetime.ApplicationStarted.Register(OnStarted);
		lifetime.ApplicationStopping.Register(OnStopping);
		lifetime.ApplicationStopped.Register(OnStopped);
	}

	private void LogThisMethod([CallerMemberName] string method = "")
	{
		_logger.LogDebug("{Method}", method);
	}

	private void OnStarted() => LogThisMethod();
	private void OnStopping() => LogThisMethod();
	private void OnStopped() => LogThisMethod();

	public Task StartAsync(CancellationToken cancellationToken)
	{
		LogThisMethod();
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		LogThisMethod();
		return Task.CompletedTask;
	}
}