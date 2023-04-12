using System.Collections;

namespace PilotLauncher.Plugins;

public interface IWorkflowNode : IEnumerable<IWorkflowNode>
{
	string Label { get; }

	string Description { get; }

	IEnumerable<IWorkflowNode> Children { get; }
}