namespace PilotLauncher.Plugins;

public interface IWorkflowNode
{
	string Label { get; }

	IEnumerable<IWorkflowNode> Children { get; }
}