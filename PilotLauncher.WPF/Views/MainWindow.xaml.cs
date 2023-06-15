using System.Reactive;
using System.Reactive.Disposables;
using PilotLauncher.PropertyGrid;
using PilotLauncher.Workflow;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;

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
			ViewModel!.Interactions.ShowEditFlyout
				.RegisterHandler(ShowEditFlyout)
				.DisposeWith(d);
			ViewModel!.Interactions.ShowConsoleOutput
				.RegisterHandler(ShowConsoleOutput)
				.DisposeWith(d);

			this.BindCommand(ViewModel,
				model => model.Interactions.ShowConsoleOutputCommand,
				window => window.ConsoleOutputButton);
			this.OneWayBind(ViewModel,
				model => model.LogOutput,
				window => window.ConsoleOutput.ItemsSource);
		});
		
		//@todo property editor sorting and grouping
	}

	private void ShowEditFlyout(InteractionContext<WorkflowNodeViewModel, Unit> context)
	{
		PropertyGridView.PropertySource = context.Input;

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