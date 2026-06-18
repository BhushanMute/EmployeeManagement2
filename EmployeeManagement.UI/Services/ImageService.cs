// File: EmployeeManagement.UI/Services/ImageService.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.UI.Services
{
    

    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageService> _logger;
        private readonly string _apiBaseUrl;

        public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger, IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:26024/";
        }

        public string GetImageUrl(string profilePath)
        {
            if (string.IsNullOrEmpty(profilePath))
            {
                return "/images/default-profile.png";
            }

            // If it's already a relative path, return as-is
            if (profilePath.StartsWith("/"))
            {
                return profilePath;
            }

            // If it's an absolute URL from the API, convert it to relative
            if (profilePath.StartsWith("http"))
            {
                try
                {
                    var uri = new Uri(profilePath);
                    // If it's from the API server, convert to relative path
                    if (uri.Host.Contains("localhost") && uri.Port == 26024)
                    {
                        // Extract the path component
                        var relativePath = uri.AbsolutePath;
                        _logger.LogInformation($"Converted API URL {profilePath} to relative path {relativePath}");
                        return relativePath;
                    }
                    else
                    {
                        // External URL - handle as needed
                        _logger.LogWarning($"External image URL detected: {profilePath}");
                        return profilePath;
                    }
                }
                catch (UriFormatException)
                {
                    _logger.LogError($"Invalid URI format: {profilePath}");
                    return "/images/default-profile.png";
                }
            }

            // If it's a simple filename, assume it's in uploads
            if (!profilePath.Contains("/") && !profilePath.Contains("\\"))
            {
                return $"/uploads/profiles/{profilePath}";
            }

            return profilePath;
        }

        public string ConvertToRelativePath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return absolutePath;

            if (absolutePath.StartsWith("http"))
            {
                try
                {
                    var uri = new Uri(absolutePath);
                    return uri.AbsolutePath;
                }
                catch
                {
                    return absolutePath;
                }
            }

            return absolutePath;
        }
    }
}