using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Spottarr.Data.Helpers;

internal static class DateConverters
{
    internal static readonly ValueConverter<DateTime, DateTime> UtcConverter
        = new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    internal static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter
        = new(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
}