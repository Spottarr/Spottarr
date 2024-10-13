using System.Globalization;
using Spottarr.Services.Parsers;

namespace Spottarr.Services.Models;

public class NntpHeader
{
    public required long ArticleNumber { get; init; }
    public required string Subject { get; init; }
    public required string Author { get; init; }
    public required DateTimeOffset Date { get; init; }
    public required string MessageId { get; init; }
    public required string References { get; init; }
    public required long Bytes { get; init; }
    public required int Lines { get; init; }
}