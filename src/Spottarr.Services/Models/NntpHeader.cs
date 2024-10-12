using System.Globalization;
using Spottarr.Services.Parsers;

namespace Spottarr.Services.Models;

public class NntpHeader
{
    private NntpHeader(long articleNumber, string subject, string author, DateTimeOffset date, string messageId,
        string references, long bytes, int lines)
    {
        ArticleNumber = articleNumber;
        Subject = subject;
        Author = author;
        Date = date;
        MessageId = messageId;
        References = references;
        Bytes = bytes;
        Lines = lines;
    }

    public long ArticleNumber { get; }
    public string Subject { get; }
    public string Author { get; }
    public DateTimeOffset Date { get; }
    public string MessageId { get; }
    public string References { get; }
    public long Bytes { get; }
    public int Lines { get; }

    public static NntpHeader Parse(string header)
    {
        ArgumentNullException.ThrowIfNull(header);

        try
        {
            // Get the first 8 fields of the header, dispose anything extra
            var fields = header.Split('\t');

            if (fields.Length < 8)
                throw new ArgumentException($"Expected 8 header fields, got {fields.Length}", nameof(header));

            var articleNumber = long.Parse(fields[0], CultureInfo.InvariantCulture);
            var date = HeaderDateParser.Parse(fields[3]);
            var bytes = fields[6].Length == 0 ? 0 : long.Parse(fields[6], CultureInfo.InvariantCulture);
            var lines = fields[7].Length == 0 ? 0 : int.Parse(fields[7], CultureInfo.InvariantCulture);
            
            return new NntpHeader(articleNumber, fields[1], fields[2], date, fields[4], fields[5], bytes, lines);
        }
        catch(Exception ex)
        {
            throw new ArgumentException($"Failed to parse Nntp header: {header}", nameof(header), ex);
        }
    }
}