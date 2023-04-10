using System.Reactive;
using System.Reactive.Disposables;
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
			ViewModel!.Interactions.ShowConsoleOutput
				.RegisterHandler(ShowConsoleOutput)
				.DisposeWith(d);

			this.BindCommand(ViewModel,
				model => model.Interactions.ShowConsoleOutputCommand,
				window => window.ConsoleOutputButton);

			this.OneWayBind(ViewModel,
				model => model.ConsoleOutput,
				window => window.ConsoleOutput.ItemsSource);

			this.BindCommand(ViewModel,
				model => model.ExecuteCommand,
				window => window.RunButton,
				model => model.WorkflowRoot);
		});
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