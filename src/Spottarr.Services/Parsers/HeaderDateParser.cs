using System.Globalization;
using System.Text.RegularExpressions;

namespace Spottarr.Services.Parsers;

/// <summary>
/// This class was taken from https://raw.githubusercontent.com/keimpema/Usenet/master/Usenet/Nntp/Parsers/HeaderDateParser.cs
/// and adjusted to pass analyzer rules
/// </summary>
internal static class HeaderDateParser
{
    /// <summary>
    /// Parses header date/time strings as described in the
    /// <a href="https://tools.ietf.org/html/rfc5322#section-3.3">Date and Time Specification</a>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static DateTimeOffset Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        
        var valueParts = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
        if (valueParts.Length > 2)
            throw new BadHeaderDateFormatException();

        // skip day-of-week for now
        //string dayOfWeek = valueParts.Length == 2 ? valueParts[0] : null;

        var dateTime = valueParts.Length == 2 ? valueParts[1] : valueParts[0];

        // remove obsolete whitespace from time part
        dateTime = Regex.Replace(dateTime, @"\s+:\s+", ":");

        var dateTimeParts = dateTime.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
        if (dateTimeParts.Length != 5 && dateTimeParts is not [_, _, _, _, _, "(UTC)"])
            throw new BadHeaderDateFormatException();

        ParseDate(dateTimeParts, out var year, out var month, out var day);
        ParseTime(dateTimeParts[3], out var hour, out var minute, out var second);
        var zone = ParseZone(dateTimeParts[4]);

        return new DateTimeOffset(year, month, day, hour, minute, second, 0, zone);
    }

    private static void ParseDate(string[] dateTimeParts, out int year, out int month, out int day)
    {
        if (dateTimeParts.Length < 3)
            throw new BadHeaderDateFormatException();
        
        if (!int.TryParse(dateTimeParts[0], out day))
            throw new BadHeaderDateFormatException();
        
        var monthString = dateTimeParts[1];
        var monthIndex = Array.FindIndex(DateTimeFormatInfo.InvariantInfo.AbbreviatedMonthNames,
            m => string.Equals(m, monthString, StringComparison.OrdinalIgnoreCase));
        
        if (monthIndex < 0)
            throw new BadHeaderDateFormatException();
        
        month = monthIndex + 1;
        if (!int.TryParse(dateTimeParts[2], out year))
            throw new BadHeaderDateFormatException();
        
        if (dateTimeParts[2].Length <= 2) 
            year += 100 * GetCentury(year, month, day);
    }

    private static int GetCentury(int year, int month, int day)
    {
        var today = DateTime.UtcNow.Date;
        var currentCentury = today.Year / 100;
        return new DateTime(currentCentury * 100 + year, month, day, 0, 0, 0, DateTimeKind.Utc) > today
            ? currentCentury - 1
            : currentCentury;
    }

    private static void ParseTime(string value, out int hour, out int minute, out int second)
    {
        var timeParts = value.Split([':'], StringSplitOptions.RemoveEmptyEntries);
        if (timeParts.Length is < 2 or > 3)
            throw new BadHeaderDateFormatException();
        
        if (!int.TryParse(timeParts[0], out hour))
            throw new BadHeaderDateFormatException();
        
        if (!int.TryParse(timeParts[1], out minute))
            throw new BadHeaderDateFormatException();
        
        second = 0;
        if (timeParts.Length > 2 && !int.TryParse(timeParts[2], out second))
            throw new BadHeaderDateFormatException();
    }

    private static TimeSpan ParseZone(string value)
    {
        // The time zone must be as specified in RFC822, https://tools.ietf.org/html/rfc822#section-5

        if (!short.TryParse(value, out var zone))
        {
            zone = ParseZoneText(value);
        }
        else if (zone is < -9999 or > 9999)
        {
            throw new BadHeaderDateFormatException();
        }

        var minute = zone % 100;
        var hour = zone / 100;
        return TimeSpan.FromMinutes(hour * 60 + minute);
    }

    private static short ParseZoneText(string value) =>
        value switch
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
            _ => throw new BadHeaderDateFormatException()
        };
}