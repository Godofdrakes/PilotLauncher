using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PilotLauncher.Services;

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

	private void OnStarted() => _logger.TraceFunction();
	private void OnStopping() => _logger.TraceFunction();
	private void OnStopped() => _logger.TraceFunction();

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.TraceFunction();
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.TraceFunction();
		return Task.CompletedTask;
	}
}