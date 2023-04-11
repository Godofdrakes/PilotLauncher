namespace PilotLauncher.Plugins;

public static class EnumerableEx
{
	public static IEnumerable<T> Flatten<T>(this T root, Func<T, IEnumerable<T>> selector)
	{
		return selector(root).SelectMany(x => x.Flatten(selector)).Prepend(root);
	}
}