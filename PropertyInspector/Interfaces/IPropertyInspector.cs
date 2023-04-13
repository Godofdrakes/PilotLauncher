namespace PropertyInspector.Interfaces;

public interface IPropertyInspector
{
	string PropertyName { get; }

	Type PropertyType { get; }
	
	bool CanWrite { get; }

	object? Value { get; set; }
}