using Microsoft.Extensions.Primitives;

namespace Spottarr.Web;

internal sealed class WriteableQueryCollection : Dictionary<string, StringValues>, IQueryCollection
{
    ICollection<string> IQueryCollection.Keys => Keys;

    public WriteableQueryCollection(IQueryCollection queryCollection) : base(queryCollection.ToDictionary(x => x.Key, x => x.Value))
    {
    }
}