using System.Collections;

namespace PilotLauncher.Common;

public interface IWorkflowNode
{
	string Label { get; }

	string Description { get; }

	IEnumerable<IWorkflowNode> Children { get; }
}