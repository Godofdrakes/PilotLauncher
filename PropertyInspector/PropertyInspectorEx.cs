using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using PropertyInspector.Attributes;
using PropertyInspector.Implementations;
using PropertyInspector.Interfaces;
using ReactiveUI;

namespace PropertyInspector;

public static class PropertyInspectorEx
{
	private const BindingFlags PublicInstanceMembers = BindingFlags.Public | BindingFlags.Instance;

	public static IEnumerable<PropertyInfo> GetPropertiesForInspection(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
		this object sourceObject)
	{
		// All instance properties
		return sourceObject.GetType().GetProperties(PublicInstanceMembers)
			// That are tagged for inspection
			.Where(info => info.GetCustomAttribute<InspectAttribute>() is not null)
			// That are readable
			.Where(info => info.CanRead);
	}

	public static IEnumerable<IPropertyInspector> CreatePropertyInspectors(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
		this ReactiveObject sourceObject)
	{
		return sourceObject.GetPropertiesForInspection()
			.Select(info => new PropertyInspectorReactive(sourceObject, info));
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