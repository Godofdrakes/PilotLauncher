using Microsoft.Extensions.Logging;

namespace PilotLauncher.WorkflowLog;

public class WorkflowLogger : ILogger
{
	private readonly string _name;
	private readonly WorkflowLoggerProvider _provider;

	public WorkflowLogger(string name, WorkflowLoggerProvider provider)
	{
		_name = name;
		_provider = provider;
	}

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		_provider.Log(_name, logLevel, eventId, state, exception, formatter);
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel is not LogLevel.None;
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return default;
	}
}