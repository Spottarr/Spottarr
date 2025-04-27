namespace Spottarr.Services.Parsers;

internal static class HeaderDateParser
{
    internal static ParserResult<DateTimeOffset> Parse(string value)
    {
        try
        {
            var result = Usenet.Nntp.Parsers.HeaderDateParser.Parse(value);
            return result == null
                ? new ParserResult<DateTimeOffset>("Header date is not in the correct format")
                : new ParserResult<DateTimeOffset>(result);
        }
        catch (InvalidOperationException ex)
        {
            return new ParserResult<DateTimeOffset>(ex.Message);
        }
    }
}