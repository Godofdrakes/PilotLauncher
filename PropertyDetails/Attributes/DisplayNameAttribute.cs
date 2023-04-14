namespace PropertyDetails.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayNameAttribute : PropertyDetailsAttribute
{
	public string Name { get; }

	public DisplayNameAttribute(string name)
	{
		Name = name;
	}
}