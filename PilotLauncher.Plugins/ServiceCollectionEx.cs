using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;

namespace PilotLauncher.Plugins;

public static class ServiceCollectionEx
{
	// This is basically a copy of the normal ReactiveUI RegisterViewsForViewModels function
	// but reworked for use with Microsoft.Extensions.DependencyInjection
	public static IServiceCollection RegisterViewsForViewModels(
		this IServiceCollection serviceCollection,
		Assembly assembly)
	{
		if (serviceCollection is null)
			throw new ArgumentNullException(nameof(serviceCollection));

		if (assembly is null)
			throw new ArgumentNullException(nameof(assembly));

		bool IsViewFor(TypeInfo typeInfo) => ImplementsViewFor(typeInfo) && !typeInfo.IsAbstract;
		bool ImplementsViewFor(TypeInfo typeInfo) => typeInfo.ImplementedInterfaces.Contains(typeof(IViewFor));

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

		return serviceCollection;
	}

	private static void RegisterType(
		IServiceCollection serviceCollection,
		TypeInfo serviceImplementation,
		Type serviceInterface)
	{
		// Microsoft.Extensions.DependencyInjection does not support named service contracts
		if (serviceImplementation.GetCustomAttribute<ViewContractAttribute>() is not null)
			throw new NotSupportedException($"{nameof(ViewContractAttribute)} is not supported");

		if (serviceImplementation.GetCustomAttribute<SingleInstanceViewAttribute>() is not null)
		{
			throw new NotSupportedException($"{nameof(SingleInstanceViewAttribute)} is not supported");
		}

		serviceCollection.AddTransient(serviceImplementation);
		serviceCollection.AddTransient(serviceInterface, provider => provider.GetRequiredService(serviceImplementation));
	}

	public static IServiceCollection RegisterHostedServices(
		this IServiceCollection serviceCollection,
		Assembly assembly)
	{
		bool IsHostedService(TypeInfo typeInfo) => typeInfo.IsSealed && typeInfo.ImplementedInterfaces
			.Contains(typeof(IHostedService));

		foreach (var typeInfo in assembly.DefinedTypes.Where(IsHostedService))
		{
			serviceCollection.AddSingleton(typeof(IHostedService), typeInfo);
		}

		return serviceCollection;
	}
}