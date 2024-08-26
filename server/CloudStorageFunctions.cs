using Elysian.Application.Exceptions;
using Elysian.Application.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MediatR;
using Elysian.Application.Features.CloudStorage.Queries;
using ElysianFunctions.Middleware;

namespace ElysianFunctions
{
    public class CloudStorageFunctions(ILogger<CloudStorageFunctions> logger, IMediator mediatr, IClaimsPrincipalAccessor claimsPrincipalAccessor)
    {
        [Function("GenerateSasToken")]
        public async Task<HttpResponseData> GenerateSasToken([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            if (!claimsPrincipalAccessor.IsAuthenticated)
            {
                throw new ForbiddenAccessException();
            }

            var fileName = req.Query["fileName"] ?? throw new CustomValidationException();

            var response = await mediatr.Send(new GenerateSasTokenQuery(fileName));

            return await req.WriteJsonResponseAsync(new
            {
                sasToken = response.SasToken,
                containerName = response.ContainerName,
                accountName = response.AccountName,
                blobName = response.BlobName,
                folderId = response.StorageId
            });
        }

    }
}
