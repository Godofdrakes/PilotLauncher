using System;
using System.Collections.Generic;

namespace PilotLauncher.PropertyGrid;

internal static class EnumerableEx
{
	public static IEnumerable<T> Flatten<T>(this T root, Func<T, T?> selector)
	{
		ArgumentNullException.ThrowIfNull(root);

		var items = new List<T>();

		var item = root;

		do
		{
#if DEBUG
			if (items.Contains(item))
			{
				throw new InvalidOperationException("Infinite loop detected");
			}
#endif

			items.Add(item);
			item = selector(item);
		}
		while (item is not null);

		return items;
	}
}