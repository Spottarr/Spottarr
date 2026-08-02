using System.Text.Json.Serialization;
using Spottarr.Web.Api.Models;

namespace Spottarr.Web.Helpers;

/// <summary>
/// Use System.Text.Json source generation for JSON serialization.
/// This is required for trimmed builds to work correctly.
/// </summary>
[JsonSerializable(typeof(SpotResponse))]
[JsonSerializable(typeof(SpotSelectionRequest))]
[JsonSerializable(typeof(SpotFlagStatusResponse))]
[JsonSerializable(typeof(SpotFlaggedResponse))]
[JsonSerializable(typeof(SpotFlagsClearedResponse))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(int?))]
internal partial class SpottarrJsonSerializerContext : JsonSerializerContext;
