using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PilotLauncher.Interfaces;

namespace PilotLauncher.Example;

public class Plugin : ILauncherPlugin
{
	public void ConfigureHost(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
	{
		serviceCollection.AddHostedService<ExampleService>();
	}
}