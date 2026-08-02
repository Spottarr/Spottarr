using Spottarr.Services.Models;
using Spottarr.Web.Api.Models;

namespace Spottarr.Web.Api;

internal static class SpotSelectionMapper
{
    public static bool TryCreateSelection(
        this SpotSelectionRequest request,
        out SpotSelection selection,
        out Dictionary<string, string[]> errors
    )
    {
        selection = new SpotSelection();
        errors = [];

        var hasSpotIds = request.SpotIds is { Count: > 0 };
        var hasDates = request.SpottedAfter.HasValue || request.SpottedBefore.HasValue;

        if (request.All)
        {
            if (hasSpotIds || hasDates)
                errors[nameof(SpotSelectionRequest.All)] =
                [
                    "'all' can not be combined with another selection.",
                ];

            return errors.Count == 0;
        }

        // Selecting every spot is expensive enough to require saying so.
        if (!hasSpotIds && !hasDates)
        {
            errors[nameof(SpotSelectionRequest.SpotIds)] =
            [
                "Provide 'spotIds', 'spottedAfter' / 'spottedBefore' or 'all'.",
            ];
            return false;
        }

        if (
            request.SpottedAfter.HasValue
            && request.SpottedBefore.HasValue
            && request.SpottedAfter > request.SpottedBefore
        )
        {
            errors[nameof(SpotSelectionRequest.SpottedAfter)] =
            [
                "'spottedAfter' must not be later than 'spottedBefore'.",
            ];
            return false;
        }

        selection = new SpotSelection
        {
            SpotIds = request.SpotIds,
            SpottedAfter = request.SpottedAfter,
            SpottedBefore = request.SpottedBefore,
        };

        return true;
    }
}
