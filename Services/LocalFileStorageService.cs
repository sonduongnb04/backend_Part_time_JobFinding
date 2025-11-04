using System.Security.Cryptography;

namespace PTJ.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadPath;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _config = config;
        _uploadPath = config["FileStorage:LocalPath"] ?? "uploads";
    }

    public async Task<FileUploadResult> UploadFileAsync(IFormFile file, string folder, Guid userId)
    {
        // Validate file
        ValidateFile(file);

        // Generate unique filename
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine(_env.ContentRootPath, _uploadPath, folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Calculate checksum
        var checksum = await CalculateChecksumAsync(filePath);

        return new FileUploadResult
        {
            StorageUrl = $"{folder}/{fileName}",
            FileName = file.FileName,
            ByteSize = file.Length,
            ContentType = file.ContentType,
            Checksum = checksum
        };
    }

    public async Task<bool> DeleteFileAsync(string storageUrl)
    {
        try
        {
            var filePath = Path.Combine(_env.ContentRootPath, _uploadPath, storageUrl);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Stream> DownloadFileAsync(string storageUrl)
    {
        var filePath = Path.Combine(_env.ContentRootPath, _uploadPath, storageUrl);
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", storageUrl);

        var memory = new MemoryStream();
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;
        return memory;
    }

    public string GetFileUrl(string storageUrl)
    {
        return $"/uploads/{storageUrl}";
    }

    private void ValidateFile(IFormFile file)
    {
        // Max file size from config (default 10MB)
        var maxSizeMB = _config.GetValue<int>("FileStorage:MaxFileSizeMB", 10);
        var maxSizeBytes = maxSizeMB * 1024 * 1024;

        if (file.Length > maxSizeBytes)
            throw new InvalidOperationException($"File size exceeds {maxSizeMB}MB limit");

        if (file.Length == 0)
            throw new InvalidOperationException("File is empty");

        // Allowed extensions from config
        var allowedExtensions = _config.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException($"File type {extension} is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
    }

    private async Task<string> CalculateChecksumAsync(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
