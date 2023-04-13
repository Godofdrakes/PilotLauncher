namespace PropertyInspector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class InspectAttribute : System.Attribute
{
	public bool CanWrite { get; }

	public InspectAttribute(bool canWrite = true)
	{
		CanWrite = canWrite;
	}
}