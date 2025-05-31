using System.Text.Json.Serialization;

namespace Spottarr.Web.Helpers;

/// <summary>
/// Use System.Text.Json source generation for JSON serialization.
/// This is required for trimmed builds to work correctly.
/// </summary>
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(int?))]
internal partial class SpottarrJsonSerializerContext : JsonSerializerContext;