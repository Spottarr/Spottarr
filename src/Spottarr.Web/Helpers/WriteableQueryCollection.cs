using System.Collections;
using Microsoft.Extensions.Primitives;

namespace Spottarr.Web.Helpers;

internal sealed class WriteableQueryCollection : IQueryCollection
{
    private readonly Dictionary<string, StringValues> _values;

    public WriteableQueryCollection(IQueryCollection queryCollection)
        => _values = queryCollection.ToDictionary(x => x.Key, x => x.Value);

    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out StringValues value) => _values.TryGetValue(key, out value);
    public int Count => _values.Count;
    ICollection<string> IQueryCollection.Keys => _values.Keys;
    public StringValues this[string key] => _values.TryGetValue(key, out var value) ? value : StringValues.Empty;
    public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
    public void Add(string key, StringValues value) => _values.Add(key, value);
    public bool Remove(string key, out StringValues value) => _values.Remove(key, out value);
}