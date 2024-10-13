using System.Globalization;
using System.Text.RegularExpressions;
using Spottarr.Services.Models;

namespace Spottarr.Services.Parsers;

public static partial class SpotnetHeaderParser
{
    public static SpotnetHeader Parse(NntpHeader header)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(header);
            
            var subjectAndTags = header.Subject.Split('|', 2,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var subject = subjectAndTags[0];
            var tag = subjectAndTags.Length == 2 ? subjectAndTags[1] : string.Empty;

            var regex = SpotnetHeaderRegex();
            var match = regex.Match(header.Author);

            if (!match.Success)
                throw new InvalidOperationException($"Invalid Spotnet Author header '{header.Author}'");

            var g = match.Groups;

            var category = int.Parse(g["cat"].Value, CultureInfo.InvariantCulture);
            var size = long.Parse(g["size"].Value, CultureInfo.InvariantCulture);
            var unixTime = long.Parse(g["date"].Value, CultureInfo.InvariantCulture);
            var date = DateTimeOffset.FromUnixTimeSeconds(unixTime);

            // Splits the sub category codes.
            // While the category is not zero-indexed, the sub categories are.
            // For example 27a00b00c07d01z00 has category 2 but subcategories A01,B01,C08,D02,Z01
            var subCategories = g["scats"].Captures
                .Select(c => (char.ToUpperInvariant(c.Value[0]), int.Parse(c.Value[1..3], CultureInfo.InvariantCulture)))
                .Where(x => x.Item2 > 0)
                .ToList();

            return new SpotnetHeader
            {
                Subject = subject,
                Tag = tag,
                Nickname = g["n"].Value,
                UserModulus = g["umod"].Value,
                UserSignature = g["usig"].Value,
                Category = category,
                KeyId = g["kid"].Value,
                SubCategories = subCategories,
                Size = size,
                Date = date,
                CustomId = g["cid"].Value,
                CustomValue = g["cv"].Value,
                ServerSignature = g["ssig"].Value,
                NntpHeader = header,
            };
        }
        catch(Exception ex)
        {
            throw new ArgumentException($"Failed to parse Spotnet header: '{header?.Author}' '{header?.Subject}'", nameof(header), ex);
        }
    }

    /*
     The Spotnet author header has the following format:
     [Nickname] <[USER-MODULUS].[USER-SIGNATURE]@[CAT][KEY-ID][SUBCATS].[SIZE].[DEPRECATED].[DATE].[CUSTOM-ID].[CUSTOM-VALUE].[SERVER-SIGNATURE]>
     See: https://github.com/spotnet/spotnet/wiki/Spot-Header-format#message-id
    */

    [GeneratedRegex(
        @"(?<n>.+) <(((?<umod>[\-0-z=]+)\.(?<usig>[\-0-z=]+))|(?<nosig>.+))@(?<cat>[0-9]{1})(?<kid>[0-9]{1})((?<scats>[abcdz][0-9]{2})*)\.(?<size>\d+)\.(?<x>\d+)\.(?<date>\d+)\.(?<cid>.+)\.(?<cv>.+)\.(?<ssig>[\-0-z=]+)>")]
    private static partial Regex SpotnetHeaderRegex();
}