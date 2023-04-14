using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using PropertyDetails.Attributes;
using PropertyDetails.Implementations;
using PropertyDetails.Interfaces;
using ReactiveUI;

namespace PropertyDetails;

public static class PropertyDetailsEx
{
	private const BindingFlags PUBLIC_INSTANCE_MEMBERS = BindingFlags.Public | BindingFlags.Instance;

	public static IEnumerable<IPropertyDetails> CreatePropertyDetails(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
		this ReactiveObject sourceObject)
	{
		return sourceObject.GetType()
			// All public instance properties
			.GetProperties(PUBLIC_INSTANCE_MEMBERS)
			// That are tagged for inspection
			.Where(info => info.GetCustomAttributes<PropertyDetailsAttribute>().Any())
			// That are readable
			.Where(info => info.CanRead)
			.Select(info => new PropertyDetailsReactive(sourceObject, info));
	}

	private static string GetPropertyValueWithFallback(this PropertyInfo propertyInfo, string fallback, Func<PropertyInfo, string?> getter)
	{
		var value = getter.Invoke(propertyInfo);
		return string.IsNullOrEmpty(value) ? fallback : value;
	}

	private static T GetPropertyValueWithFallback<T>(this PropertyInfo propertyInfo, T fallback, Func<PropertyInfo, T?> getter)
		where T : struct
	{
		return getter.Invoke(propertyInfo) ?? fallback;
	}

	public static string GetPropertyName(this PropertyInfo propertyInfo)
	{
		return propertyInfo.GetPropertyValueWithFallback(propertyInfo.Name, info =>
		{
			return info.GetCustomAttribute<DisplayNameAttribute>()?.Name;
		});
	}

	public static bool GetPropertyCanWrite(this PropertyInfo propertyInfo)
	{
		// Property cannot be written to
		if (!propertyInfo.CanWrite)
			return false;

		// Property is not publicly writable
		if (propertyInfo.SetMethod?.IsPublic != true)
			return false;

		return propertyInfo.GetPropertyValueWithFallback(propertyInfo.CanWrite, info =>
		{
			return info.GetCustomAttribute<InspectAttribute>()?.CanWrite;
		});
	}
}