using System;
using System.Collections.Generic;
using DynamicData;

namespace PilotLauncher.WPF;

public static class SourceListEx
{
	public static void ReplaceAll<T>(this ISourceList<T> source, IEnumerable<T> enumerable)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(enumerable);

		source.Edit(list =>
		{
			list.Clear();
			list.AddRange(enumerable);
		});
	}
}