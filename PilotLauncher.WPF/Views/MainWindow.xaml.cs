using System.Linq;
using System.Reactive;
using PilotLauncher.Plugins;
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
		});
	}

	private void ShowEditFlyout(InteractionContext<IWorkflowNode, Unit> context)
	{
		if (context.Input is ReactivePrototypeObject prototype)
		{
			EditWorkflowDataGrid.ItemsSource = prototype.GetExposedProperties();
		}
		else
		{
			EditWorkflowDataGrid.ItemsSource = Enumerable.Empty<ReactivePropertyInfo>();
		}

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