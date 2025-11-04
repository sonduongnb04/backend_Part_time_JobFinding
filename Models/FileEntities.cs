using PTJ.Auth;

namespace PTJ.Core;

public class FileEntity
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long ByteSize { get; set; }
    public string StorageUrl { get; set; } = default!;
    public string StorageProvider { get; set; } = default!;
    public string? Checksum { get; set; }
    public Guid? OwnerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation
    public User? OwnerUser { get; set; }
}
