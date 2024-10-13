using System.Globalization;
using Spottarr.Services.Models;

namespace Spottarr.Services.Parsers;

public static class NntpHeaderParser
{
    public static NntpHeader Parse(string header)
    {
        ArgumentNullException.ThrowIfNull(header);

        // TODO: Use response of overview format response to determine fields
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
            
            return new NntpHeader
            {
                ArticleNumber = articleNumber,
                Subject = fields[1],
                Author = fields[2],
                Date = date,
                MessageId = fields[4],
                References = fields[5],
                Bytes = bytes,
                Lines = lines
            };
            
        }
        catch(Exception ex)
        {
            throw new ArgumentException($"Failed to parse Nntp header: {header}", nameof(header), ex);
        }
    }
}