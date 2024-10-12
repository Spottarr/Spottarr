namespace Spottarr.Services.Parsers;

public sealed class BadHeaderDateFormatException : FormatException
{
    public BadHeaderDateFormatException() : base("Header date is not in the correct format")
    {
            
    }

    public BadHeaderDateFormatException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public BadHeaderDateFormatException(string message) : base(message)
    {
    }
}