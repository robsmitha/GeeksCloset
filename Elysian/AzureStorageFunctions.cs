using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Elysian.Application.Exceptions;
using Elysian.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Elysian
{
    public class AzureStorageFunctions(ILogger<AzureStorageFunctions> logger, BlobServiceClient blobServiceClient,
        IConfiguration configuration, IClaimsPrincipalAccessor claimsPrincipalAccessor)
    {
        [Function("GenerateSasToken")]
        public async Task<HttpResponseData> GenerateSasToken([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            if (!claimsPrincipalAccessor.IsAuthenticated)
            {
                throw new ForbiddenAccessException();
            }

            var folderId = Guid.NewGuid();
            var fileName = req.Query["fileName"] ?? throw new CustomValidationException();

            var containerClient = await GetBlobContainerClient("cards");
            var blobName = $"GeeksCloset/{folderId}/{fileName}";
            var sasToken = await CreateSasTokenAsync(containerClient, blobName, 
                BlobSasPermissions.Create | BlobSasPermissions.Add | BlobSasPermissions.Write);
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; charset=utf-8");
            await response.WriteStringAsync(JsonConvert.SerializeObject(new
            {
                sasToken,
                containerName = containerClient.Name,
                accountName = containerClient.AccountName,
                blobName,
                folderId
            }));
            return response;
        }

        private async Task<BlobContainerClient> GetBlobContainerClient(string container)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(container);

            var createResponse = await containerClient.CreateIfNotExistsAsync();
            if (createResponse != null)
            {
                await CreateAccessPolicyAsync(containerClient);
            }

            return containerClient;
        }

        private async Task CreateAccessPolicyAsync(BlobContainerClient containerClient)
        {
            var policy = await containerClient.GetAccessPolicyAsync();
            var permissions = policy.Value.SignedIdentifiers.ToList();

            var set = false;
            if(permissions.All(p => p.Id != "AdminAccessPolicy"))
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerClient.Name,
                    // Container
                    Resource = "c",
                    Identifier = "AdminAccessPolicy"
                };

                sasBuilder.SetPermissions(BlobAccountSasPermissions.All);
                permissions.Add(new BlobSignedIdentifier
                {
                    Id = sasBuilder.Identifier,
                    AccessPolicy = new BlobAccessPolicy
                    {
                        Permissions = sasBuilder.Permissions,
                    }
                });

                set = true;
            }

            if (permissions.All(p => p.Id != "PublicAccessPolicy"))
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerClient.Name,
                    // Container
                    Resource = "c",
                    Identifier = "PublicAccessPolicy"
                };

                sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
                permissions.Add(new BlobSignedIdentifier
                {
                    Id = sasBuilder.Identifier,
                    AccessPolicy = new BlobAccessPolicy
                    {
                        Permissions = sasBuilder.Permissions,
                    }
                });

                set = true;
            }

            if (set)
            {
                await containerClient.SetAccessPolicyAsync(permissions: permissions);
            }
        }
        // Uploading
        private async Task<string> CreateSasTokenAsync(BlobContainerClient containerClient,
            string blobName, BlobSasPermissions permissions)
        {
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync() && permissions.HasFlag(BlobSasPermissions.Read))
            {
                throw new NotFoundException();
            }

            var now = DateTime.UtcNow;
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobName,
                // Container
                Resource = "c",
                StartsOn = now.AddMinutes(-5),
                ExpiresOn = now.AddHours(24)
            };
            sasBuilder.SetPermissions(permissions);

            var accountKey = configuration.GetSection("Azure:AccountKey").Get<string>();
            var sasToken = sasBuilder.ToSasQueryParameters(
                new StorageSharedKeyCredential(blobServiceClient.AccountName, accountKey)).ToString();

            return sasToken;
        }
    }
}
