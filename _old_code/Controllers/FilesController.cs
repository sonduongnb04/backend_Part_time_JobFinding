using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Data;
using PTJ.Services;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public FilesController(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    // GET /api/files/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var file = await _db.Files
            .Where(f => f.FileId == id && !f.IsDeleted)
            .FirstOrDefaultAsync();

        if (file == null)
            return NotFound(new { message = "File not found" });

        try
        {
            var stream = await _fileStorage.DownloadFileAsync(file.StorageUrl);
            return File(stream, file.ContentType, file.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { message = "File not found in storage" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error downloading file", error = ex.Message });
        }
    }

    // GET /api/files/{id}/info
    [HttpGet("{id}/info")]
    public async Task<IActionResult> GetFileInfo(Guid id)
    {
        var file = await _db.Files
            .Where(f => f.FileId == id && !f.IsDeleted)
            .FirstOrDefaultAsync();

        if (file == null)
            return NotFound(new { message = "File not found" });

        return Ok(new
        {
            fileId = file.FileId,
            fileName = file.FileName,
            contentType = file.ContentType,
            byteSize = file.ByteSize,
            url = _fileStorage.GetFileUrl(file.StorageUrl),
            createdAt = file.CreatedAt
        });
    }
}
