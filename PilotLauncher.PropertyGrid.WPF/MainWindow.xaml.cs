using System.Reflection;
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
		var declaringType = eventArgs.PropertyInfo.DeclaringType;
		if (declaringType is not null && declaringType.Assembly != Assembly.GetExecutingAssembly())
		{
			// Skip properties declared in library types
			eventArgs.Cancel = true;
		}
	}
}