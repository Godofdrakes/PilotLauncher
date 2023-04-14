namespace PilotLauncher.WPF.Design;

public class PropertyDetailsViewModel : ViewModels.PropertyDetailsViewModel
{
	public PropertyDetailsViewModel()
	{
		WorkflowNode = new WorkflowStepExample
		{
			Delay = 5,
			Message = "Hello World!"
		};
	}
}