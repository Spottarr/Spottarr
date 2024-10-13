using System.Text.RegularExpressions;

namespace Spottarr.Services.Models;

public partial class SpotnetHeader
{
    private SpotnetHeader(string subject, string tag, string nickname, string userModulus,
        string userSignature, string category, string keyId, string subCategory, string size, string random,
        string date, string customId, string customValue, string serverSignature, NntpHeader nntpHeader)
    {
        Subject = subject;
        Tag = tag;
        Nickname = nickname;
        UserModulus = userModulus;
        UserSignature = userSignature;
        Category = category;
        KeyId = keyId;
        SubCategory = subCategory;
        Size = size;
        Random = random;
        Date = date;
        CustomId = customId;
        CustomValue = customValue;
        ServerSignature = serverSignature;
        NntpHeader = nntpHeader;
    }

    public string Subject { get; }
    public string Tag { get; }
    public string Nickname { get; }
    public string UserModulus { get; }
    public string UserSignature { get; }
    public string Category { get; }
    public string KeyId { get; }
    public string SubCategory { get; }
    public string Size { get; }
    public string Random { get; }
    public string Date { get; }
    public string CustomId { get; }
    public string CustomValue { get; }
    public string ServerSignature { get; }
    public NntpHeader NntpHeader { get; }

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

        return new SpotnetHeader(subject, tag, g["n"].Value, g["umod"].Value, g["usig"].Value, g["cat"].Value, g["kid"].Value,
            g["scats"].Value, g["size"].Value, g["x"].Value, g["date"].Value, g["sid"].Value, g["cv"].Value,
            g["ssig"].Value, header);
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