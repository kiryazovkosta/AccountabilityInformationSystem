using AccountabilityInformationSystem.Api.Domain.Entities.Common;

namespace AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;

public interface IFileStorage
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, bool isPrivate, CancellationToken cancellationToken);
    Uri GetReadUrl(string blobName, bool isPrivate);
    Task DeleteAsync(string blobName, bool isPrivate, CancellationToken cancellationToken);
    Task<StorageFile?> UploadAsStorageFileAsync(IFormFile? file, string username, bool isPrivate = true, CancellationToken cancellationToken = default);
}
