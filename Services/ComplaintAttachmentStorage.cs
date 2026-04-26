using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ConsumersVoiceSystemPrototype.Services;

public class ComplaintAttachmentStorage(Cloudinary cloudinary)
{
    private const long MaxBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".doc", ".docx", ".xlsx", ".txt"
    };

    public long MaxFileSizeBytes => MaxBytes;

    public bool IsAllowedExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(ext) && AllowedExtensions.Contains(ext);
    }

    public async Task<(string StoredFileName, string RelativeWebPath, string ContentType, long Size)?> SaveAsync(
        int complaintId,
        IFormFile file,
        CancellationToken ct = default)
    {
        if (file.Length <= 0 || file.Length > MaxBytes) return null;
        if (!IsAllowedExtension(file.FileName)) return null;

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType = string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType;
        var publicId = $"complaints/{complaintId}/{Guid.NewGuid():N}";
        var isImage = ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp";

        using var stream = file.OpenReadStream();
        string publicUrl;
        string storedId;

        if (isImage)
        {
            var result = await cloudinary.UploadAsync(new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Overwrite = false
            }, ct);
            if (result.Error != null) return null;
            publicUrl = result.SecureUrl.ToString();
            storedId = result.PublicId;
        }
        else
        {
            var result = await cloudinary.UploadAsync(new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Overwrite = false
            }, "upload", ct);
            if (result.Error != null) return null;
            publicUrl = result.SecureUrl.ToString();
            storedId = result.PublicId;
        }

        return (storedId, publicUrl, contentType, file.Length);
    }
}
