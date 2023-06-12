using System;
using System.Reactive.Disposables;
using System.Threading;

namespace PilotLauncher.PropertyGrid;

public static class ManualResetEventSlimEx
{
	public static IDisposable ScopedSet(this ManualResetEventSlim manualResetEventSlim)
	{
		manualResetEventSlim.Set();
		return Disposable.Create(manualResetEventSlim.Reset);
	}
}