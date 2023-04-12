namespace PilotLauncher.WPF.Design;

public class EditWorkflowViewModel : ViewModels.EditWorkflowViewModel
{
	public EditWorkflowViewModel()
	{
		WorkflowNode = new WorkflowStepExample
		{
			Delay = 5,
			Message = "Hello World!"
		};
	}
}