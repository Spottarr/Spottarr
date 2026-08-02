using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Spottarr.Web.Api.Models;

namespace Spottarr.Web.Helpers;

/// <summary>
/// Use System.Text.Json source generation for JSON serialization.
/// This is required for trimmed builds to work correctly.
/// </summary>
[JsonSerializable(typeof(SpotResponse))]
[JsonSerializable(typeof(SpotSelectionRequest))]
[JsonSerializable(typeof(MarkedSpotsStatusResponse))]
[JsonSerializable(typeof(MarkedSpotsResponse))]
[JsonSerializable(typeof(UnmarkedSpotsResponse))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(HttpValidationProblemDetails))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(int?))]
internal partial class SpottarrJsonSerializerContext : JsonSerializerContext;
