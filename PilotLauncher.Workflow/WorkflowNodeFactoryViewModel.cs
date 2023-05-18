using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace PilotLauncher.Workflow;

public class WorkflowNodeFactoryViewModel : ReactiveObject
{
	public ReactiveCommand<Type, WorkflowNodeViewModel> CreateNodeCommand { get; }

	public WorkflowNodeFactoryViewModel(IServiceProvider serviceProvider)
	{
		CreateNodeCommand = ReactiveCommand.Create((Type type) =>
			(WorkflowNodeViewModel)ActivatorUtilities.CreateInstance(serviceProvider, type));
	}
}