using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Elysian.Application.Exceptions;
using Elysian.Application.Extensions;
using Elysian.Application.Interfaces;
using Elysian.Domain.Data;
using Elysian.Infrastructure.Context;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Elysian
{
    public class CardFunctions(ILogger<CardFunctions> logger, ElysianContext context, IClaimsPrincipalAccessor claimsPrincipalAccessor, 
        BlobServiceClient blobServiceClient)
    {
        [Function("GetCards")]
        public async Task<HttpResponseData> GetCards([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            // TODO: Protect endpoints with AuthorizationLevel.User
            // AuthorizationLevel settings do not work in development.
            // After publishing to Azure, the authLevel setting is enforced.the authLevel setting is enforced.
            if (!claimsPrincipalAccessor.IsAuthenticated)
            {
                throw new ForbiddenAccessException();
            }

            var cards = await context.Cards.Where(c => !c.IsDeleted).ToListAsync();
            return await req.WriteJsonResponseAsync(cards);
        }

        [Function("GetCard")]
        public async Task<HttpResponseData> GetCard([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            // TODO: Protect endpoints with AuthorizationLevel.User
            // AuthorizationLevel settings do not work in development.
            // After publishing to Azure, the authLevel setting is enforced.
            if (!claimsPrincipalAccessor.IsAuthenticated)
            {
                throw new ForbiddenAccessException();
            }

            if (!int.TryParse(req.Query["cardId"], out var cardId))
            {
                throw new CustomValidationException();
            }

            logger.LogInformation("Get Card by Id request [CardId: {cardId}]", cardId);

            var (card, images) = await GetCardExtensionsAsync(c => !c.IsDeleted && c.CardId == cardId);

            return card == null
                ? throw new NotFoundException()
                : await req.WriteJsonResponseAsync(new
                {
                    card,
                    images
                });
        }

        [Function("GetCardBySerialNumber")]
        public async Task<HttpResponseData> GetCardBySerialNumber([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var serialNumber = req.Query["serialNumber"];
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new CustomValidationException();
            }

            logger.LogInformation("Get Card By Serial Number request [SerialNumber: {serialNumber}]", serialNumber);

            var (card, images) = await GetCardExtensionsAsync(c => !c.IsDeleted && c.SerialNumber == serialNumber);

            var containerClient = blobServiceClient.GetBlobContainerClient("cards");
            var sasUris = new List<Uri>();
            foreach (var image in images)
            {
                var sasUri = await GetSasUriAsync(containerClient, "GeeksCloset", image.StorageId, image.FileName, BlobSasPermissions.Read);
                sasUris.Add(sasUri);
            }
            return card == null
                ? throw new NotFoundException()
                : await req.WriteJsonResponseAsync(new
                {
                    card,
                    sasUris
                });
        }

        private async Task<(Card, List<CardImage>)> GetCardExtensionsAsync(Expression<Func<Card, bool>> predicate)
        {
            var card = await context.Cards.SingleOrDefaultAsync(predicate) ?? throw new NotFoundException();
            var images = await context.CardImages.Where(i => i.CardId == card.CardId && !i.IsDeleted).ToListAsync();
            return (card, images);
        }

        private async Task<Uri> GetSasUriAsync(BlobContainerClient containerClient,
            string containerPrefix, Guid folderId,
            string fileName, BlobSasPermissions permissions)
        {
            var blobName = $"{containerPrefix}/{folderId}/{fileName}";
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

            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri;
        }


        public record SaveCardRequest(string Name, string Description, string SerialNumber, 
            string Grade, List<SaveCardImage> AddImages, int? CardId = null);
        public record SaveCardImage(string FileName, long FileSize, Guid StorageId);
        [Function("SaveCard")]
        public async Task<HttpResponseData> SaveCard([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            // TODO: Protect endpoints with AuthorizationLevel.User
            // AuthorizationLevel settings do not work in development.
            // After publishing to Azure, the authLevel setting is enforced.
            if (!claimsPrincipalAccessor.IsAuthenticated)
            {
                throw new ForbiddenAccessException();
            }

            var saveCard = await req.DeserializeBodyAsync<SaveCardRequest>();
            if (!saveCard.CardId.HasValue && await context.Cards.AnyAsync(c => !c.IsDeleted && c.SerialNumber == saveCard.SerialNumber))
            {
                throw new CustomValidationException();
            }

            if (saveCard.CardId.HasValue && await context.Cards.AnyAsync(c => !c.IsDeleted && c.SerialNumber == saveCard.SerialNumber && c.CardId != saveCard.CardId))
            {
                throw new CustomValidationException();
            }

            var card = saveCard.CardId.HasValue
                ? await context.Cards.SingleOrDefaultAsync(c => c.CardId == saveCard.CardId)
                : null;

            if (card == null)
            {
                card = new Card
                {
                    SerialNumber = saveCard.SerialNumber,
                    Name = saveCard.Name,
                    Description = saveCard.Description,
                    Grade = saveCard.Grade,
                    CreatedByUserId = claimsPrincipalAccessor.UserId,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedByUserId = claimsPrincipalAccessor.UserId,
                    ModifiedAt = DateTime.UtcNow
                };
                context.Add(card);
            }
            else
            {
                card.SerialNumber = saveCard.SerialNumber;
                card.Name = saveCard.Name;
                card.Grade = saveCard.Grade;
                card.Description = saveCard.Description;
                card.ModifiedByUserId = claimsPrincipalAccessor.UserId;
                card.ModifiedAt = DateTime.UtcNow;
            }
            await context.SaveChangesAsync();

            foreach (var image in saveCard.AddImages)
            {
                context.Add(new CardImage
                {
                    CardId = card.CardId,
                    FileName = image.FileName,
                    FileSize = image.FileSize,
                    AltText = image.FileName,
                    StorageId = image.StorageId,
                    ModifiedByUserId = claimsPrincipalAccessor.UserId,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedByUserId = claimsPrincipalAccessor.UserId,
                    CreatedAt = DateTime.UtcNow,
                });
            }
            await context.SaveChangesAsync();

            return await req.WriteJsonResponseAsync(card);
        }
        public record DeleteCardRequest(int CardId);
        [Function("DeleteCard")]
        public async Task<HttpResponseData> DeleteCard([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            // TODO: Protect endpoints with AuthorizationLevel.User
            // AuthorizationLevel settings do not work in development.
            // After publishing to Azure, the authLevel setting is enforced.
            if (!claimsPrincipalAccessor.IsAuthenticated)
            {
                throw new ForbiddenAccessException();
            }

            var deleteCard = await req.DeserializeBodyAsync<DeleteCardRequest>();
            logger.LogInformation("Delete Card by Id request [CardId: {cardId}]", deleteCard.CardId);

            var (card, images) = await GetCardExtensionsAsync(c => c.CardId == deleteCard.CardId);
            card.IsDeleted = true;
            images.ForEach(i => i.IsDeleted = true);
            await context.SaveChangesAsync();

            // TODO: Create timer trigger that deletes from azure

            return await req.WriteJsonResponseAsync(new
            {
                success = true
            });
        }
        public record DeleteCardImageRequest(int CardImageId);
        [Function("DeleteCardImage")]
        public async Task<HttpResponseData> DeleteCardImage([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            // TODO: Protect endpoints with AuthorizationLevel.User
            // AuthorizationLevel settings do not work in development.
            // After publishing to Azure, the authLevel setting is enforced.
            if (!claimsPrincipalAccessor.IsAuthenticated)
            {
                throw new ForbiddenAccessException();
            }

            var deleteCardImage = await req.DeserializeBodyAsync<DeleteCardImageRequest>();
            logger.LogInformation("Delete Card Image by Id request [CardImageId: {CardImageId}]", deleteCardImage.CardImageId);

            var image = await context.CardImages.SingleOrDefaultAsync(i => i.CardImageId == deleteCardImage.CardImageId && !i.IsDeleted) ?? throw new NotFoundException();
            image.IsDeleted = true;
            await context.SaveChangesAsync();

            return await req.WriteJsonResponseAsync(new
            {
                success = true
            });
        }
    }
}
