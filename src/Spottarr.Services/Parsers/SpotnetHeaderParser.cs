using System.Text.RegularExpressions;
using Spottarr.Services.Models;

namespace Spottarr.Services.Parsers;

public static partial class SpotnetHeaderParser
{
    public static SpotnetHeader Parse(NntpHeader header)
    {
        ArgumentNullException.ThrowIfNull(header);

        var subjectAndTags = header.Subject.Split('|', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var subject = subjectAndTags[0];
        var tag = subjectAndTags.Length == 2 ? subjectAndTags[1] : string.Empty;

        var regex = SpotnetHeaderRegex();
        var match = regex.Match(header.Author);

        if (!match.Success)
            throw new InvalidOperationException($"Invalid Spotnet Author header '{header.Author}'");

        var g = match.Groups;

        return new SpotnetHeader
        {
            Subject = subject,
            Tag = tag,
            Nickname = g["n"].Value,
            UserModulus = g["umod"].Value,
            UserSignature = g["usig"].Value,
            Category = g["cat"].Value,
            KeyId = g["kid"].Value,
            SubCategory = g["scats"].Value,
            Size = g["size"].Value,
            Date = g["date"].Value,
            CustomId = g["cid"].Value,
            CustomValue = g["cv"].Value,
            ServerSignature = g["ssig"].Value,
            NntpHeader = header,
        };
    }

    /*
     The Spotnet author header has the following format:
     [Nickname] <[USER-MODULUS].[USER-SIGNATURE]@[CAT][KEY-ID][SUBCATS].[SIZE].[DEPRECATED].[DATE].[CUSTOM-ID].[CUSTOM-VALUE].[SERVER-SIGNATURE]>
     See: https://github.com/spotnet/spotnet/wiki/Spot-Header-format#message-id
    */

    [GeneratedRegex(
        @"(?<n>.+) <(((?<umod>[\-0-z=]+)\.(?<usig>[\-0-z=]+))|(?<nosig>.+))@(?<cat>[0-9]{1})(?<kid>[0-9]{1})(?<scats>([abcdz][0-9]{2})*)\.(?<size>\d+)\.(?<x>\d+)\.(?<date>\d+)\.(?<cid>.+)\.(?<cv>.+)\.(?<ssig>[\-0-z=]+)>")]
    private static partial Regex SpotnetHeaderRegex();
}