using Spottarr.Services.Nntp;

namespace Spottarr.Tests;

public class NntpArticleRangeTests
{
    [Fact]
    public void CheckRange()
    {
        var result = NntpArticleRangeFactory.GetBatches(10, 2110, 1000);
        Assert.Collection(result, r1 =>
        {
            Assert.Equal(1111, r1.From);
            Assert.Equal(2110, r1.To);
        }, r2 =>
        {
            Assert.Equal(111, r2.From);
            Assert.Equal(1110, r2.To);
        }, r3 =>
        {
            Assert.Equal(10, r3.From);
            Assert.Equal(110, r3.To);
        });
    }
}