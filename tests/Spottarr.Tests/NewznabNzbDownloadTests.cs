using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;
using Spottarr.Web.Endpoints;

namespace Spottarr.Tests;

internal sealed class NewznabNzbDownloadTests
{
    [Test]
    public async Task AcceptsEitherParameterAndPreservesGuidPrecedence()
    {
        foreach (
            var (guid, id) in new (int? Guid, string? Id)[]
            {
                (null, "6337"),
                (6337, null),
                (6337, "6337"),
                (6337, "6338"),
                (6337, "invalid"),
            }
        )
        {
            var attachmentService = new FakeSpotnetAttachmentService();

            var result = await NewznabEndpoints.GetNzbHandler(
                guid,
                id,
                attachmentService,
                CancellationToken.None
            );

            await Assert.That(attachmentService.RequestedSpotId).IsEqualTo(6337);
            await Assert
                .That(((IStatusCodeHttpResult)result).StatusCode)
                .IsEqualTo(StatusCodes.Status404NotFound);
        }
    }

    [Test]
    public async Task RejectsMissingOrMalformedId()
    {
        foreach (var id in new string?[] { null, "invalid" })
        {
            var attachmentService = new FakeSpotnetAttachmentService();

            var result = await NewznabEndpoints.GetNzbHandler(
                null,
                id,
                attachmentService,
                CancellationToken.None
            );

            await Assert
                .That(((IStatusCodeHttpResult)result).StatusCode)
                .IsEqualTo(StatusCodes.Status400BadRequest);
            await Assert.That(attachmentService.RequestedSpotId).IsNull();
        }
    }

    [Test]
    public async Task HandlesMalformedQueryParameters()
    {
        var attachmentService = new FakeSpotnetAttachmentService();
        using var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<ISpotnetAttachmentService>(attachmentService)
            .BuildServiceProvider();
        var endpoint = RequestDelegateFactory.Create(
            (Delegate)NewznabEndpoints.GetNzbHandler,
            new RequestDelegateFactoryOptions { ServiceProvider = services }
        );

        foreach (var query in new[] { "?id=invalid", "?guid=invalid" })
        {
            var context = new DefaultHttpContext { RequestServices = services };
            context.Request.QueryString = new QueryString(query);

            await endpoint.RequestDelegate(context);

            await Assert
                .That(context.Response.StatusCode)
                .IsEqualTo(StatusCodes.Status400BadRequest);
        }

        await Assert.That(attachmentService.RequestedSpotId).IsNull();

        var legacyContext = new DefaultHttpContext { RequestServices = services };
        legacyContext.Request.QueryString = new QueryString("?guid=6337&id=invalid");

        await endpoint.RequestDelegate(legacyContext);

        await Assert
            .That(legacyContext.Response.StatusCode)
            .IsEqualTo(StatusCodes.Status404NotFound);
        await Assert.That(attachmentService.RequestedSpotId).IsEqualTo(6337);
    }

    [Test]
    public async Task PreservesSuccessfulDownloadResponse()
    {
        var attachmentService = new FakeSpotnetAttachmentService
        {
            Result = new SpotAttachmentResponse
            {
                FileName = "release",
                Stream = new MemoryStream(),
            },
        };

        var result = await NewznabEndpoints.GetNzbHandler(
            6337,
            null,
            attachmentService,
            CancellationToken.None
        );
        var fileResult = (IFileHttpResult)result;

        await Assert.That(fileResult.ContentType).IsEqualTo("application/x-nzb");
        await Assert.That(fileResult.FileDownloadName).IsEqualTo("release.nzb");
    }

    private sealed class FakeSpotnetAttachmentService : ISpotnetAttachmentService
    {
        public int? RequestedSpotId { get; private set; }
        public SpotAttachmentResponse? Result { get; init; }

        public Task<SpotAttachmentResponse?> FetchNzb(
            int spotId,
            CancellationToken cancellationToken
        )
        {
            RequestedSpotId = spotId;
            return Task.FromResult(Result);
        }

        public Task<SpotAttachmentResponse?> FetchImage(
            int spotId,
            CancellationToken cancellationToken
        ) => throw new NotSupportedException();
    }
}
