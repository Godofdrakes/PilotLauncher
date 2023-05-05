using ReactiveUI;

namespace PilotLauncher.PropertyGrid.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public Person Person { get; } = new();

	public MainWindow()
	{
		InitializeComponent();

		PropertyGrid.PropertySource = Person;
	}

	private void PropertyGrid_OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs eventArgs)
	{
		eventArgs.Cancel = typeof(ReactiveObject).IsAssignableTo(eventArgs.PropertyInfo.DeclaringType);
	}
}