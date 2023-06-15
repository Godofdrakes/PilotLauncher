using System.Windows.Interactivity;
using JetBrains.Annotations;

namespace PilotLauncher.PropertyGrid;

[PublicAPI]
public abstract class PropertyGridFilter : Behavior<PropertyGridView>
{
	protected override void OnAttached() => AssociatedObject.PropertyItemAdded += OnPropertyItemAdded;
	protected override void OnDetaching() => AssociatedObject.PropertyItemAdded -= OnPropertyItemAdded;

	protected abstract void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e);
}