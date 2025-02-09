using System.Diagnostics.CodeAnalysis;

namespace Spottarr.Services.Parsers;

public readonly record struct ParserResult<TResult>
{
    private readonly TResult? _result = default;
    private readonly string? _error = null;
    public ParserResult(TResult result) => _result = result;
    public ParserResult(string error) => _error = error;

    [MemberNotNullWhen(true, nameof(_error))]
    [MemberNotNullWhen(false, nameof(_result))]
    [MemberNotNullWhen(false, nameof(Result))]
    public bool HasError => _error is not null;

    public string Error => HasError ? _error : string.Empty;

    public TResult Result => !HasError ? _result : throw new InvalidOperationException($"Parsing was not successful: {_error}");
}