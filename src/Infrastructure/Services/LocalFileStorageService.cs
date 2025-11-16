using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PTJ.Application.Interfaces;
using PTJ.Domain.Constants;
using PTJ.Domain.Entities;
using PTJ.Domain.Interfaces;

namespace PTJ.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadPath;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;

    public LocalFileStorageService(
        IWebHostEnvironment env,
        IConfiguration config,
        IUnitOfWork unitOfWork)
    {
        _env = env;
        _config = config;
        _unitOfWork = unitOfWork;
        _uploadPath = config["FileStorage:LocalPath"] ?? "uploads";
    }

    public async Task<FileEntity> UploadFileAsync(IFormFile file, Guid? ownerUserId)
    {
        // Validate file
        ValidateFile(file);

        // Generate unique filename
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";

        // Determine folder based on file type
        var folder = DetermineFolder(file.ContentType);
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

        // Create file entity
        var fileEntity = new FileEntity
        {
            FileId = Guid.NewGuid(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            ByteSize = file.Length,
            StorageUrl = $"{folder}/{fileName}",
            StorageProvider = FileConstants.StorageProvider.Local,
            Checksum = checksum,
            OwnerUserId = ownerUserId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _unitOfWork.Files.AddAsync(fileEntity);
        await _unitOfWork.SaveChangesAsync();

        return fileEntity;
    }

    public async Task<Stream> DownloadFileAsync(Guid fileId)
    {
        var fileEntity = await _unitOfWork.Files.GetByIdAsync(fileId);
        if (fileEntity == null || fileEntity.IsDeleted)
            throw new FileNotFoundException("File not found");

        var filePath = Path.Combine(_env.ContentRootPath, _uploadPath, fileEntity.StorageUrl);
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Physical file not found", fileEntity.StorageUrl);

        var memory = new MemoryStream();
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;
        return memory;
    }

    public async Task DeleteFileAsync(Guid fileId)
    {
        var fileEntity = await _unitOfWork.Files.GetByIdAsync(fileId);
        if (fileEntity == null)
            throw new FileNotFoundException("File not found");

        // Soft delete
        fileEntity.IsDeleted = true;
        _unitOfWork.Files.Update(fileEntity);
        await _unitOfWork.SaveChangesAsync();

        // Optionally delete physical file
        try
        {
            var filePath = Path.Combine(_env.ContentRootPath, _uploadPath, fileEntity.StorageUrl);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            // Log error but don't throw - metadata is already marked as deleted
        }
    }

    public async Task<FileEntity?> GetFileMetadataAsync(Guid fileId)
    {
        return await _unitOfWork.Files.GetByIdAsync(fileId);
    }

    private void ValidateFile(IFormFile file)
    {
        // Max file size
        if (file.Length > FileConstants.MaxFileSizeBytes)
            throw new InvalidOperationException($"File size exceeds {FileConstants.MaxFileSizeBytes / 1024 / 1024}MB limit");

        if (file.Length == 0)
            throw new InvalidOperationException("File is empty");

        // Allowed extensions
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allAllowedExtensions = FileConstants.AllowedImageExtensions
            .Concat(FileConstants.AllowedDocumentExtensions)
            .ToArray();

        if (!allAllowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                $"File type {extension} is not allowed. Allowed types: {string.Join(", ", allAllowedExtensions)}");
    }

    private string DetermineFolder(string contentType)
    {
        if (contentType.StartsWith("image/"))
            return "images";
        if (contentType.Contains("pdf") || contentType.Contains("document"))
            return "documents";
        return "others";
    }

    private async Task<string> CalculateChecksumAsync(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
