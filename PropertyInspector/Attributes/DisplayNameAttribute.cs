namespace PropertyInspector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayNameAttribute : PropertyInspectorAttribute
{
	public string Name { get; }

	public DisplayNameAttribute(string name)
	{
		Name = name;
	}
}