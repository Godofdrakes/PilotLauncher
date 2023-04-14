namespace PropertyDetails.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayFormatAttribute : PropertyDetailsAttribute
{
	public string Format { get; }

	public DisplayFormatAttribute(string format)
	{
		Format = format;
	}
}