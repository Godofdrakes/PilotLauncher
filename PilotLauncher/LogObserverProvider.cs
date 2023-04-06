using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PilotLauncher;

public class LogObservable
{
	public LogObservable(IObservable<string> output)
	{
		Output = output;
	}

	public IObservable<string> Output { get; }
}

public class LogObserver : ILogger
{
	private readonly IObserver<string> _observer;
	private readonly LogLevel _logLevel;

	public LogObserver(IObserver<string> observer, LogLevel logLevel = LogLevel.Trace)
	{
		_observer = observer;
		_logLevel = logLevel;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (IsEnabled(logLevel)) _observer.OnNext(formatter.Invoke(state, exception));
	}

	public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}

public class LogObserverProvider : ILoggerProvider
{
	private readonly IObserver<string> _observer;

	public LogObserverProvider(IObserver<string> observer)
	{
		_observer = observer;
	}

	public ILogger CreateLogger(string categoryName)
	{
		return new LogObserver(_observer);
	}

	public void Dispose() { }
}

public static class LoggingBuilderEx
{
	public static ILoggingBuilder AddObserver(this ILoggingBuilder loggingBuilder, IObserver<string> observer)
	{
		loggingBuilder.Services.AddSingleton(typeof(ILoggerProvider), provider =>
			new LogObserverProvider(observer));

		return loggingBuilder;
	}
}