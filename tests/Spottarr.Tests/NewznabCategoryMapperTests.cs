using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Newznab;

namespace Spottarr.Tests;

internal sealed class NewznabCategoryMapperTests
{
    [Test]
    public async Task MapBookIncludesBooksMagsForMagazineGenre()
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Book],
            ImageGenres = [ImageGenre.Magazine],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Books, NewznabCategory.BooksMags]);
    }

    [Test]
    public async Task MapBookDoesNotIncludeBooksMagsForNonMagazineGenre()
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Book],
            ImageGenres = [ImageGenre.History],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Books]);
    }
}
