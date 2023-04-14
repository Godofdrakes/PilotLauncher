namespace PropertyDetails.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class InspectAttribute : PropertyDetailsAttribute
{
	public bool CanWrite { get; }

	public InspectAttribute(bool canWrite = true)
	{
		CanWrite = canWrite;
	}
}