using DynamicData;
using Microsoft.Extensions.Logging;

namespace PilotLauncher.WorkflowLog;

public class WorkflowLogEntry
{
	public LogLevel LogLevel { get; }
	public EventId EventId { get; }
	public string Message { get; }
	public string Source { get; }

	public WorkflowLogEntry(
		LogLevel logLevel,
		EventId eventId,
		string message,
		string source)
	{
		LogLevel = logLevel;
		EventId = eventId;
		Message = message;
		Source = source;
	}
}

public interface IWorkflowLog
{
	IObservable<IChangeSet<WorkflowLogEntry>> Connect();
}