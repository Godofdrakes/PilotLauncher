using System.Reflection;
using System.Windows.Interactivity;

namespace PilotLauncher.PropertyGrid.WPF;

public class ExcludeLibraryTypesBehavior : Behavior<PropertyGrid>
{
	private static Assembly ThisAssembly { get; } = Assembly.GetExecutingAssembly();

	protected override void OnAttached() => AssociatedObject.PropertyItemAdded += OnPropertyItemAdded;
	protected override void OnDetaching() => AssociatedObject.PropertyItemAdded -= OnPropertyItemAdded;

	private static void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		var declaringType = e.PropertyInfo.DeclaringType?.Assembly;
		if (declaringType != Assembly.GetExecutingAssembly())
		{
			e.Cancel = true;
		}
	}
}