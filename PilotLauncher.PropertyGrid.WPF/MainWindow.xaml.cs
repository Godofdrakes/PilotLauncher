using System.Reflection;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public Person Person { get; } = new();

	public MainWindow() => InitializeComponent();
}