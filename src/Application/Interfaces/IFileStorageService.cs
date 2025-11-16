using Microsoft.AspNetCore.Http;
using PTJ.Domain.Entities;

namespace PTJ.Application.Interfaces;

public interface IFileStorageService
{
    Task<FileEntity> UploadFileAsync(IFormFile file, Guid? ownerUserId);
    Task<Stream> DownloadFileAsync(Guid fileId);
    Task DeleteFileAsync(Guid fileId);
    Task<FileEntity?> GetFileMetadataAsync(Guid fileId);
}
