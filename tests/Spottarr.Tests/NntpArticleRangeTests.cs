using Spottarr.Services.Nntp;

namespace Spottarr.Tests;

internal sealed class NntpArticleRangeTests
{
    [Test]
    public async Task RangeIsAlwaysInclusive()
    {
        var result = NntpArticleRangeFactory.GetBatches(10, 3010, 1000).ToList();
        await Assert.That(result).Count().IsEqualTo(3);

        await Assert.That(result[0].From).IsEqualTo(10);
        await Assert.That(result[0].To).IsEqualTo(1009);

        await Assert.That(result[1].From).IsEqualTo(1010);
        await Assert.That(result[1].To).IsEqualTo(2009);

        await Assert.That(result[2].From).IsEqualTo(2010);
        await Assert.That(result[2].To).IsEqualTo(3010);
    }

    [Test]
    public async Task RangeIsClampedToEnd()
    {
        var result = NntpArticleRangeFactory.GetBatches(10, 2015, 1000).ToList();
        await Assert.That(result).Count().IsEqualTo(3);

        await Assert.That(result[0].From).IsEqualTo(10);
        await Assert.That(result[0].To).IsEqualTo(1009);

        await Assert.That(result[1].From).IsEqualTo(1010);
        await Assert.That(result[1].To).IsEqualTo(2009);

        await Assert.That(result[2].From).IsEqualTo(2010);
        await Assert.That(result[2].To).IsEqualTo(2015);
    }
}
