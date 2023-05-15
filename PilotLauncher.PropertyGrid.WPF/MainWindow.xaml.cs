using System.Reflection;

namespace PilotLauncher.PropertyGrid.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public Example Example { get; } = new();

	public MainWindow() => InitializeComponent();

	private void PropertyGridView_OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		var declaringType = e.PropertyInfo.DeclaringType?.Assembly;
		if (declaringType != ThisAssembly)
		{
			e.Cancel = true;
		}
	}

	private static Assembly ThisAssembly { get; } = Assembly.GetExecutingAssembly();
}