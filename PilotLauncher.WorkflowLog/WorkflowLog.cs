using DynamicData;

namespace PilotLauncher.WorkflowLog;

public class WorkflowLog : IWorkflowLog
{
	public SourceList<WorkflowLogEntry> History { get; } = new();

	public IObservable<IChangeSet<WorkflowLogEntry>> Connect() => History.Connect();
}