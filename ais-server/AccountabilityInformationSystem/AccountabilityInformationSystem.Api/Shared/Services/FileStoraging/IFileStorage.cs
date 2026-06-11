namespace AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;

public interface IFileStorage
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, bool isPrivate, CancellationToken cancellationToken);
    Uri GetReadUrl(string blobName, bool isPrivate);
    Task DeleteAsync(string blobName, bool isPrivate, CancellationToken cancellationToken);
}
