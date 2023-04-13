namespace PropertyInspector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class InspectAttribute : PropertyInspectorAttribute
{
	public bool CanWrite { get; }

	public InspectAttribute(bool canWrite = true)
	{
		CanWrite = canWrite;
	}
}