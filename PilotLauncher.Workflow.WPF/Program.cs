using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PilotLauncher.WorkflowLog;
using PilotLauncher.WPF.Common;

namespace PilotLauncher.Workflow.WPF;

public static class Program
{
	[STAThread]
	public static Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.FixConsoleLogging()
			.ConfigureLogging()
			.ConfigureLogging(builder =>
			{
				builder.AddWorkflowLog(options =>
				{
					options.HistorySize = 10000;
				});
			})
			.ConfigureWpf<App>();

		return host
			.Build()
			.RunAsync();
	}
}