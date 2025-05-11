namespace Spottarr.Services.Parsers;

internal static class HeaderDateParser
{
    internal static ParserResult<DateTimeOffset> Parse(string value)
    {
        try
        {
            var result = Usenet.Nntp.Parsers.HeaderDateParser.Parse(value);
            return result != null
                ? new ParserResult<DateTimeOffset>(result.Value)
                : new ParserResult<DateTimeOffset>($"Header date '{value}' is not in the correct format");
        }
        catch (FormatException fex)
        {
            return new ParserResult<DateTimeOffset>(fex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return new ParserResult<DateTimeOffset>(ex.Message);
        }
    }
}