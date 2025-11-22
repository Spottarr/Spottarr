using NpgsqlTypes;

namespace Spottarr.Data.Entities.Fts;

public interface IPostgreSqlFtsEntity
{
    public NpgsqlTsVector SearchVector { get; set; }
}