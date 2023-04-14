namespace PropertyDetails.Interfaces;

public interface IPropertyDetails
{
	string PropertyName { get; }

	Type PropertyType { get; }
	
	bool CanWrite { get; }

	object? Value { get; set; }
}