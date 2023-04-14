using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PilotLauncher.Common;

public interface IPlugin
{
	void ConfigureServices(HostBuilderContext context, IServiceCollection collection);
}