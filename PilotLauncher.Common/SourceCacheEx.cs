using DynamicData;

namespace PilotLauncher.Common;

public static class SourceCacheEx
{
	public static void ReplaceAll<TObject, TKey>(this ISourceCache<TObject, TKey> source, IEnumerable<TObject> enumerable)
		where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(enumerable);

		source.Edit(updater =>
		{
			updater.Clear();
			updater.AddOrUpdate(enumerable);
		});
	}
}