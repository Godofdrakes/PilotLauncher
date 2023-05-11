namespace PilotLauncher.PropertyGrid.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public Example Example { get; } = new();

	public MainWindow() => InitializeComponent();
}