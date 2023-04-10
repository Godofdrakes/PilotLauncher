using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PilotLauncher.Plugins;

public interface IPlugin
{
	void ConfigureServices(HostBuilderContext context, IServiceCollection collection);
}