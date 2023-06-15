using JetBrains.Annotations;
using PilotLauncher.Common;

namespace PilotLauncher.PropertyGrid;

// Exclude properties declared in the specified type
[PublicAPI]
public class PropertyGridDeclaringTypeFilter : PropertyGridTypeFilter
{
	protected override void OnPropertyItemAdded(object sender, PropertyGridItemAddedEventArgs e)
	{
		Ensure.That(() => TargetType != null);

		var declaringType = e.PropertyInfo.DeclaringType;

		if (declaringType is null)
		{
			return;
		}

		if (declaringType == TargetType
			|| (FilterAssignableTo && declaringType.IsAssignableTo(TargetType))
			|| (FilterAssignableFrom && declaringType.IsAssignableFrom(TargetType)))
		{
			e.Cancel = true;
		}
	}
}