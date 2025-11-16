namespace PTJ.Domain.Constants;

public static class FileConstants
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    public static readonly string[] AllowedDocumentExtensions = { ".pdf", ".doc", ".docx" };

    public static class StorageProvider
    {
        public const string Local = "local";
        public const string AzureBlob = "azure";
    }
}
