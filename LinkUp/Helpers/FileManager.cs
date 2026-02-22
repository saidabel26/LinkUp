using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LinkUp.Helpers
{
    public interface IFileManager
    {
        Task<string?> SaveAsync(IFormFile file, string subfolder = "uploads", string[]? allowedContentTypes = null, long maxBytes = 0, CancellationToken ct = default);
        Task<bool> DeleteAsync(string? relativePath);
    }

    public class FileManager : IFileManager
    {
        private readonly IWebHostEnvironment _env;
        public FileManager(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveAsync(IFormFile file, string subfolder = "uploads", string[]? allowedContentTypes = null, long maxBytes = 0, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) return null;
            if (allowedContentTypes != null && allowedContentTypes.Length > 0)
            {
                if (!allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
                    return null;
            }
            if (maxBytes > 0 && file.Length > maxBytes) return null;

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var targetDir = Path.Combine(webRoot, subfolder);
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
            {
                ext = file.ContentType?.ToLowerInvariant() switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/gif" => ".gif",
                    "image/webp" => ".webp",
                    _ => ".bin"
                };
            }
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(targetDir, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            return $"/{subfolder}/{fileName}";
        }

        public Task<bool> DeleteAsync(string? relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath)) return Task.FromResult(false);
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var trim = relativePath!.TrimStart('/', '\\');
                var full = Path.Combine(webRoot, trim.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(full))
                {
                    File.Delete(full);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
