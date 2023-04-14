using System.Reactive;
using PilotLauncher.Common;
using ReactiveUI;

namespace PilotLauncher.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public MainWindow()
	{
		InitializeComponent();

		this.WhenActivated(d =>
		{
			d(ViewModel!.Interactions.ShowEditFlyout.RegisterHandler(ShowEditFlyout));
			d(ViewModel!.Interactions.ShowConsoleOutput.RegisterHandler(ShowConsoleOutput));

			this.BindCommand(ViewModel,
				model => model.Interactions.ShowConsoleOutputCommand,
				window => window.ConsoleOutputButton);

			this.OneWayBind(ViewModel,
				model => model.ConsoleOutput,
				window => window.ConsoleOutput.ItemsSource);

			this.OneWayBind(ViewModel,
				model => model.PropertyDetailsViewModel,
				window => window.EditWorkflowView.ViewModel);
		});
	}

	private void ShowEditFlyout(InteractionContext<IWorkflowNode, Unit> context)
	{
		ViewModel!.PropertyDetailsViewModel.WorkflowNode = context.Input;

		EditWorkflowFlyout.IsOpen = true;

		context.SetOutput(Unit.Default);
	}

	private void ShowConsoleOutput(InteractionContext<ShowConsoleOutputMode, Unit> context)
	{
		var shouldOpen = true;

		if (context.Input == ShowConsoleOutputMode.Toggle)
		{
			shouldOpen = !ConsoleOutputFlyout.IsOpen;
		}

		ConsoleOutputFlyout.IsOpen = shouldOpen;

		context.SetOutput(Unit.Default);
	}
}