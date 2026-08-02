using Spottarr.Services.Models;
using Spottarr.Web.Api.Models;

namespace Spottarr.Web.Api;

internal static class SpotSelectionMapper
{
    public static bool TryCreateSelection(
        this SpotSelectionRequest request,
        out SpotSelection selection,
        out string error
    )
    {
        selection = new SpotSelection();
        error = string.Empty;

        var hasSpotIds = request.SpotIds is { Count: > 0 };
        var hasDates = request.SpottedAfter.HasValue || request.SpottedBefore.HasValue;

        // Selecting every spot is expensive enough to require saying so.
        if (request.All)
        {
            if (hasSpotIds || hasDates)
            {
                error = "'all' can not be combined with another selection.";
                return false;
            }

            return true;
        }

        if (!hasSpotIds && !hasDates)
        {
            error = "Provide 'spotIds', 'spottedAfter' / 'spottedBefore' or 'all'.";
            return false;
        }

        if (
            request.SpottedAfter.HasValue
            && request.SpottedBefore.HasValue
            && request.SpottedAfter > request.SpottedBefore
        )
        {
            error = "'spottedAfter' must not be later than 'spottedBefore'.";
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
