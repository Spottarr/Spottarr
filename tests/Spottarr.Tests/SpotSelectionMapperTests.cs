using Spottarr.Web.Api;
using Spottarr.Web.Api.Models;

namespace Spottarr.Tests;

internal sealed class SpotSelectionMapperTests
{
    [Test]
    public async Task RejectsEmptySelection()
    {
        var valid = new SpotSelectionRequest().TryCreateSelection(out _, out var errors);

        await Assert.That(valid).IsFalse();
        await Assert.That(errors).IsNotEmpty();
    }

    [Test]
    public async Task RejectsEmptySpotIds()
    {
        var valid = new SpotSelectionRequest { SpotIds = [] }.TryCreateSelection(out _, out _);

        await Assert.That(valid).IsFalse();
    }

    [Test]
    public async Task RejectsAllCombinedWithAnotherSelection()
    {
        var valid = new SpotSelectionRequest { All = true, SpotIds = [1] }.TryCreateSelection(
            out _,
            out var errors
        );

        await Assert.That(valid).IsFalse();
        await Assert.That(errors).IsNotEmpty();
    }

    [Test]
    public async Task RejectsInvertedDateRange()
    {
        var valid = new SpotSelectionRequest
        {
            SpottedAfter = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero),
            SpottedBefore = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
        }.TryCreateSelection(out _, out _);

        await Assert.That(valid).IsFalse();
    }

    [Test]
    public async Task SelectsEverythingWithoutCriteria()
    {
        var valid = new SpotSelectionRequest { All = true }.TryCreateSelection(
            out var selection,
            out _
        );

        await Assert.That(valid).IsTrue();
        await Assert.That(selection.SpotIds).IsNull();
        await Assert.That(selection.SpottedAfter).IsNull();
        await Assert.That(selection.SpottedBefore).IsNull();
    }

    [Test]
    public async Task MapsSpotIds()
    {
        var valid = new SpotSelectionRequest { SpotIds = [1, 2] }.TryCreateSelection(
            out var selection,
            out _
        );

        await Assert.That(valid).IsTrue();
        await Assert.That(selection.SpotIds).IsEquivalentTo([1, 2]);
    }

    [Test]
    public async Task MapsDateRange()
    {
        var after = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var before = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);

        var valid = new SpotSelectionRequest
        {
            SpottedAfter = after,
            SpottedBefore = before,
        }.TryCreateSelection(out var selection, out _);

        await Assert.That(valid).IsTrue();
        await Assert.That(selection.SpottedAfter).IsEqualTo(after);
        await Assert.That(selection.SpottedBefore).IsEqualTo(before);
    }
}
