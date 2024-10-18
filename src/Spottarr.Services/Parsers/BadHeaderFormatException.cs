namespace Spottarr.Services.Parsers;

public sealed class BadHeaderFormatException : FormatException
{
    public string Header { get; }

    public BadHeaderFormatException() : base("Header is not in the correct format")
    {
        Header = string.Empty;
    }

    public BadHeaderFormatException(string header, Exception innerException) : base("Header is not in the correct format", innerException)
    {
        Header = header;
    }

    public BadHeaderFormatException(string header) : base("Header is not in the correct format")
    {
        Header = header;
    }
}