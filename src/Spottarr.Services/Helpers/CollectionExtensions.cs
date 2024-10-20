namespace Spottarr.Services.Helpers;

internal static class CollectionExtensions
{
    public static ICollection<T> Replace<T>(this ICollection<T> target, IEnumerable<T> newValues)
    {
        target.Clear();
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