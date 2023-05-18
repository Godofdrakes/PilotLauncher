using System.Collections.Concurrent;
using DynamicData;
using Microsoft.Extensions.Logging;

namespace PilotLauncher.WorkflowLog;

public class WorkflowLoggerProvider : ILoggerProvider
{
	private readonly WorkflowLog _workflowLog;
	private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

	public WorkflowLoggerProvider(WorkflowLog workflowLog)
	{
		_workflowLog = workflowLog;
	}

	public void Log<TState>(
		string source,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		_workflowLog.History.Add(new WorkflowLogEntry(
			logLevel,
			eventId,
			formatter(state, exception),
			source));
	}

	public ILogger CreateLogger(string categoryName)
	{
		return _loggers.GetOrAdd(categoryName, name =>
			new WorkflowLogger(name, this));
	}

	public void Dispose() => _loggers.Clear();
}