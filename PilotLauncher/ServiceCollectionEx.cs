using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace PilotLauncher;

public static class ServiceCollectionEx
{
	private static bool IsViewFor(TypeInfo typeInfo) => ImplementsViewFor(typeInfo) && !typeInfo.IsAbstract;
	private static bool ImplementsViewFor(TypeInfo typeInfo) => typeInfo.ImplementedInterfaces.Contains(typeof(IViewFor));

	public static void RegisterViewsForViewModels(this IServiceCollection serviceCollection, Assembly assembly)
	{
		if (serviceCollection is null)
			throw new ArgumentNullException(nameof(serviceCollection));

		if (assembly is null)
			throw new ArgumentNullException(nameof(assembly));

		// for each type that implements IViewFor
		foreach (var type in assembly.DefinedTypes.Where(IsViewFor))
		{
			// grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
			var serviceType = type.ImplementedInterfaces
				.Select(IntrospectionExtensions.GetTypeInfo)
				.FirstOrDefault(ImplementsViewFor);

			// need to check for null because some classes may implement IViewFor but not IViewFor<T> - we don't care about those
			if (serviceType is null)
				continue;

			RegisterType(serviceCollection, type, serviceType);
		}
	}

	private static void RegisterType(IServiceCollection serviceCollection, TypeInfo ti, Type serviceType)
	{
		// @todo: contract support?
		if (ti.GetCustomAttribute<ViewContractAttribute>() is not null)
			throw new NotSupportedException("view contracts not supported");

		if (ti.GetCustomAttribute<SingleInstanceViewAttribute>() is not null)
		{
			serviceCollection.AddSingleton(ti);
		}
		else
		{
			serviceCollection.AddTransient(ti);
		}

		serviceCollection.AddTransient(serviceType, provider => provider.GetRequiredService(ti));
	}
}