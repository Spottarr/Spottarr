using System.Text.RegularExpressions;

namespace Spottarr.Services.Models;

public class SpotnetHeader
{
    public required string Subject { get; init; }
    public required string Tag { get; init; }
    public required string Nickname { get; init; }
    public required string UserModulus { get; init; }
    public required string UserSignature { get; init; }
    public required int Category { get; init; }
    public required string KeyId { get; init; }
    public required IReadOnlyList<(char Type, int Code)> SubCategories { get; init; }
    public required long Size { get; init; }
    public required DateTimeOffset Date { get; init; }
    public required string CustomId { get; init; }
    public required string CustomValue { get; init; }
    public required string ServerSignature { get; init; }
    public required NntpHeader NntpHeader { get; init; }
}