using System.Globalization;
using System.Text.RegularExpressions;

namespace Spottarr.Services.Parsers;

/// <summary>
/// This class was taken from https://raw.githubusercontent.com/keimpema/Usenet/master/Usenet/Nntp/Parsers/HeaderDateParser.cs
/// and adjusted to pass analyzer rules
/// </summary>
internal static partial class HeaderDateParser
{
    /// <summary>
    /// Parses header date/time strings as described in the
    /// <a href="https://tools.ietf.org/html/rfc5322#section-3.3">Date and Time Specification</a>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static ParserResult<DateTimeOffset> Parse(string value)
    {
        var valueParts = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
        if (valueParts.Length > 2)
            return new ParserResult<DateTimeOffset>("Header date is not in the correct format");

        // skip day-of-week for now
        //string dayOfWeek = valueParts.Length == 2 ? valueParts[0] : null;

        var dateTime = valueParts.Length == 2 ? valueParts[1] : valueParts[0];

        // remove obsolete whitespace from time part
        dateTime = DateWhitespaceRegex().Replace(dateTime, ":");

        var dateTimeParts = dateTime.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
        if (dateTimeParts.Length != 5 && dateTimeParts is not [_, _, _, _, _, "(UTC)"])
            return new ParserResult<DateTimeOffset>("Header date is not in the correct format");

        var validDate = TryParseDate(dateTimeParts, out var year, out var month, out var day);
        var validTime = TryParseTime(dateTimeParts[3], out var hour, out var minute, out var second);
        var validZone = TryParseZone(dateTimeParts[4], out var zone);
        
        if(!validDate || !validTime || !validZone)
            return new ParserResult<DateTimeOffset>("Header date is not in the correct format");

        return new ParserResult<DateTimeOffset>(new DateTimeOffset(year, month, day, hour, minute, second, 0, zone));
    }

    private static bool TryParseDate(string[] dateTimeParts, out int year, out int month, out int day)
    {
        year = 0;
        month = 0;
        day = 0;
        
        if (dateTimeParts.Length < 3)
            return false;
        
        if (!int.TryParse(dateTimeParts[0], out day))
            return false;
        
        var monthString = dateTimeParts[1];
        var monthIndex = Array.FindIndex(DateTimeFormatInfo.InvariantInfo.AbbreviatedMonthNames,
            m => string.Equals(m, monthString, StringComparison.OrdinalIgnoreCase));

        if (monthIndex < 0)
            return false;
        
        month = monthIndex + 1;
        if (!int.TryParse(dateTimeParts[2], out year))
            return false;
        
        if (dateTimeParts[2].Length <= 2) 
            year += 100 * GetCentury(year, month, day);

        return true;
    }

    private static int GetCentury(int year, int month, int day)
    {
        var today = DateTime.UtcNow.Date;
        var currentCentury = today.Year / 100;
        return new DateTime(currentCentury * 100 + year, month, day, 0, 0, 0, DateTimeKind.Utc) > today
            ? currentCentury - 1
            : currentCentury;
    }

    private static bool TryParseTime(string value, out int hour, out int minute, out int second)
    {
        hour = 0;
        minute = 0;
        second = 0;
        
        var timeParts = value.Split([':'], StringSplitOptions.RemoveEmptyEntries);
        if (timeParts.Length is < 2 or > 3)
            return false;
        
        if (!int.TryParse(timeParts[0], out hour))
            return false;
        
        if (!int.TryParse(timeParts[1], out minute))
            return false;

        if (timeParts.Length > 2 && !int.TryParse(timeParts[2], out second))
            return false;

        return true;
    }

    private static bool TryParseZone(string value, out TimeSpan result)
    {
        // The time zone must be as specified in RFC822, https://tools.ietf.org/html/rfc822#section-5
        result = TimeSpan.Zero;
        
        if (!short.TryParse(value, out var zone) && !TryParseZoneText(value, out zone))
            return false;

        if (zone is < -9999 or > 9999)
            return false;

        var minute = zone % 100;
        var hour = zone / 100;
        
        result = TimeSpan.FromMinutes(hour * 60 + minute);
        return true;
    }

    private static bool TryParseZoneText(string value, out short zone)
    {
        zone = value switch
        {
            // UTC is not specified in RFC822, but allowing it since it is commonly used
            "UTC" or "UT" or "GMT" or "Z" => 0000,
            "EDT" => -0400,
            "EST" or "CDT" => -0500,
            "CST" or "MDT" => -0600,
            "MST" or "PDT" => -0700,
            "PST" => -0800,
            "A" => -0100,
            "N" => +0100,
            "M" => -1200,
            "Y" => +1200,
            _ => 9999
        };

        return zone != 9999;
    }

    [GeneratedRegex(@"\s+:\s+")]
    private static partial Regex DateWhitespaceRegex();
}