namespace PTJ.Services;

public interface IFileStorageService
{
    Task<FileUploadResult> UploadFileAsync(IFormFile file, string folder, Guid userId);
    Task<bool> DeleteFileAsync(string storageUrl);
    Task<Stream> DownloadFileAsync(string storageUrl);
    string GetFileUrl(string storageUrl);
}

public class FileUploadResult
{
    public string StorageUrl { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public long ByteSize { get; set; }
    public string ContentType { get; set; } = default!;
    public string? Checksum { get; set; }
}
