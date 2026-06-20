namespace Spottarr.Services.Helpers;

internal static class CollectionExtensions
{
    public static ICollection<T> Replace<T>(this ICollection<T> target, IEnumerable<T> newValues)
    {
        target.Clear();
        // Spot collections are all initialised as List<T> ([]), so this fast path is normally taken;
        // the fallback below keeps the helper correct for any other ICollection<T> implementation.
        if (target is List<T> listTarget)
        {
            listTarget.AddRange(newValues);
            return target;
        }

        foreach (var value in newValues)
        {
            target.Add(value);
        }

        return target;
    }
}
