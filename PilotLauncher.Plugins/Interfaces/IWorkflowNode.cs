using System.Collections;

namespace PilotLauncher.Plugins;

public interface IWorkflowNode : IEnumerable<IWorkflowNode>
{
	string Label { get; }

	IEnumerable<IWorkflowNode> Children { get; }
}