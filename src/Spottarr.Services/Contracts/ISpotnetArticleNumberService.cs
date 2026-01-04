namespace Spottarr.Services.Contracts;

public interface ISpotnetArticleNumberService
{
    Task<long> GetArticleNumberByDate(CancellationToken cancellationToken);
}