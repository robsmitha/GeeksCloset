using Elysian.Application.Exceptions;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using MediatR;
using Elysian.Application.Features.CloudStorage.Queries;
using ElysianFunctions.Middleware;

namespace ElysianFunctions
{
    public class CloudStorageFunctions(IMediator mediator)
    {
        [Function("GenerateSasToken")]
        public async Task<HttpResponseData> GenerateSasToken([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var fileName = req.Query["fileName"] ?? throw new CustomValidationException();

            var response = await mediator.Send(new GenerateSasTokenQuery(fileName));

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
