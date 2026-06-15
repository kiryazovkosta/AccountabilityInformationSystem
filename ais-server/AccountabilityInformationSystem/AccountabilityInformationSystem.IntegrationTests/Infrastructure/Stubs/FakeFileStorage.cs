using AccountabilityInformationSystem.Api.Domain.Entities.Common;
using AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;
using Microsoft.AspNetCore.Http;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure.Stubs;

internal sealed class FakeFileStorage : IFileStorage
{
    public Task<string> UploadAsync(Stream content, string fileName, string contentType, bool isPrivate, CancellationToken cancellationToken) =>
        Task.FromResult($"fake_{Guid.NewGuid():N}{Path.GetExtension(fileName)}");

    public Uri GetReadUrl(string blobName, bool isPrivate) =>
        new($"https://fake-storage.local/{(isPrivate ? "private" : "public")}/{blobName}");

    public Task DeleteAsync(string blobName, bool isPrivate, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task<StorageFile?> UploadAsStorageFileAsync(IFormFile? file, string username, bool isPrivate = true, CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            return Task.FromResult<StorageFile?>(null);
        }

        return Task.FromResult<StorageFile?>(new StorageFile
        {
            Id = $"sf_{Guid.CreateVersion7()}",
            BlobName = $"fake_{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}",
            IsPrivate = isPrivate,
            OriginalFileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            CreatedBy = username,
            CreatedAt = DateTime.UtcNow
        });
    }
}
