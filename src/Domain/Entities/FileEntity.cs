using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTJ.Domain.Entities;

[Table("Files", Schema = "core")]
public class FileEntity
{
    [Key] public Guid FileId { get; set; }
    [MaxLength(500)] public string FileName { get; set; } = default!;
    [MaxLength(200)] public string ContentType { get; set; } = default!;
    public long ByteSize { get; set; }
    [MaxLength(1000)] public string StorageUrl { get; set; } = default!;
    [MaxLength(50)] public string StorageProvider { get; set; } = default!;
    [MaxLength(64)] public string? Checksum { get; set; }
    public Guid? OwnerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public User? OwnerUser { get; set; }
}
