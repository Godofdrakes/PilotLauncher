using System.Windows;
using PilotLauncher.Common;
using PilotLauncher.WPF.Common;

namespace PilotLauncher.PropertyGrid;

// Exclude properties declared in the specified assembly
public class PropertyGridDeclaringAssemblyFilter : PropertyGridFilter
{
	public static readonly DependencyProperty TargetAssemblyProperty = DependencyObjectEx
		.RegisterProperty((PropertyGridDeclaringAssemblyFilter filter) => filter.TargetAssembly);

	public string TargetAssembly
	{
		get => (string)GetValue(TargetAssemblyProperty);
		set => SetValue(TargetAssemblyProperty, value);
	}

	protected override void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		Ensure.That(() => !string.IsNullOrEmpty(TargetAssembly));

		var declaringType = e.PropertyInfo.DeclaringType;
		var assemblyName = declaringType?.Assembly.GetName();
		if (assemblyName?.Name?.StartsWith(TargetAssembly) is true)
		{
			e.Cancel = true;
		}
	}
}