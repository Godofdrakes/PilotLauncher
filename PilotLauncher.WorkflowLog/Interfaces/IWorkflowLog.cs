using DynamicData;
using Microsoft.Extensions.Logging;

namespace PilotLauncher.WorkflowLog;

public class WorkflowLogEntry
{
	public LogLevel LogLevel { get; }
	public EventId EventId { get; }
	public DateTime DateTime { get; }
	public string Message { get; }
	public string Source { get; }

	public string MinimalSource
	{
		// LastIndexOf might return -1 but -1 + 1 is 0 so it's fine
		get { return Source[(Source.LastIndexOf('.') + 1)..]; }
	}

	public WorkflowLogEntry(
		LogLevel logLevel,
		EventId eventId,
		DateTime dateTime,
		string message,
		string source)
	{
		LogLevel = logLevel;
		EventId = eventId;
		DateTime = dateTime;
		Message = message;
		Source = source;
	}
}

public interface IWorkflowLog
{
	IObservable<IChangeSet<WorkflowLogEntry>> Connect();
}