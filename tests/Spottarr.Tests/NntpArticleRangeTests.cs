using Spottarr.Services.Nntp;

namespace Spottarr.Tests;

public class NntpArticleRangeTests
{
    [Fact]
    public void RangeIsAlwaysInclusive()
    {
        var result = NntpArticleRangeFactory.GetBatches(10, 3010, 1000);
        Assert.Collection(result, r1 =>
        {
            Assert.Equal(10, r1.From);
            Assert.Equal(1009, r1.To);
        }, r2 =>
        {
            Assert.Equal(1010, r2.From);
            Assert.Equal(2009, r2.To);
        }, r3 =>
        {
            Assert.Equal(2010, r3.From);
            Assert.Equal(3010, r3.To);
        });
    }
    
    [Fact]
    public void RangeIsClampedToEnd()
    {
        var result = NntpArticleRangeFactory.GetBatches(10, 2015, 1000);
        Assert.Collection(result, r1 =>
        {
            Assert.Equal(10, r1.From);
            Assert.Equal(1009, r1.To);
        }, r2 =>
        {
            Assert.Equal(1010, r2.From);
            Assert.Equal(2009, r2.To);
        }, r3 =>
        {
            Assert.Equal(2010, r3.From);
            Assert.Equal(2015, r3.To);
        });
    }
}