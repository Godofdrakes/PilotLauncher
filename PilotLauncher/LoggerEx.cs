using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace PilotLauncher;

public static class LoggerEx
{
	public static void TraceFunction(this ILogger logger, [CallerMemberName] string method = "")
	{
		logger.LogTrace("{Method}", method);
	}
}