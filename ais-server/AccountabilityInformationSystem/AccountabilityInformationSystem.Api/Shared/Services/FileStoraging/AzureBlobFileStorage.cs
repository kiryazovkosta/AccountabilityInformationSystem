using System.Reflection.Metadata;
using AccountabilityInformationSystem.Api.Domain.Entities.Common;
using AccountabilityInformationSystem.Api.Settings;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using static System.Reflection.Metadata.BlobBuilder;

namespace AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;

public sealed class AzureBlobFileStorage(
    BlobServiceClient blobService,
    IOptions<AzureStorageOptions> options) : IFileStorage
{
    private readonly AzureStorageOptions _options = options.Value;

    private string ContainerFor(bool isPrivate) => 
        isPrivate ? _options.PrivateContainer : _options.PublicContainer;

    public async Task<string> UploadAsync(
        Stream content, 
        string fileName, 
        string contentType, 
        bool isPrivate, 
        CancellationToken cancellationToken)
    {
        BlobContainerClient bliobContainer = blobService.GetBlobContainerClient(ContainerFor(isPrivate));

        string blobName = $"af_{Guid.CreateVersion7():N}{Path.GetExtension(fileName)}";
        BlobClient blobClient = bliobContainer.GetBlobClient(blobName);

        await blobClient.UploadAsync(content,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return blobName;
    }

    public Uri GetReadUrl(string blobName, bool isPrivate)
    {
        BlobClient blobClient = blobService.GetBlobContainerClient(ContainerFor(isPrivate)).GetBlobClient(blobName);

        TimeSpan lifetime = isPrivate
            ? TimeSpan.FromMinutes(_options.PrivateSasLifetimeMinutes)
            : TimeSpan.FromDays(_options.PublicSasLifetimeDays);

        BlobSasBuilder? sas = new(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(lifetime))
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name
        };

        return blobClient.GenerateSasUri(sas);
    }

    public Task DeleteAsync(string blobName, bool isPrivate, CancellationToken cancellationToken)
    {
        return blobService.GetBlobContainerClient(ContainerFor(isPrivate))
            .GetBlobClient(blobName)
            .DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<StorageFile?> UploadAsStorageFileAsync(
        IFormFile? file, 
        string username, 
        bool isPrivate = true, 
        CancellationToken cancellationToken = default)
    {
        if (file == null)
        {
            return null;
        }

        await using Stream stream = file.OpenReadStream();
        string blobName = await UploadAsync(
            stream, file.FileName, file.ContentType, isPrivate, cancellationToken);

        return new StorageFile
        {
            Id = $"sf_{Guid.CreateVersion7()}",
            BlobName = blobName,
            IsPrivate = isPrivate,
            OriginalFileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            CreatedBy = username,
            CreatedAt = DateTime.UtcNow
        };
    }
}
