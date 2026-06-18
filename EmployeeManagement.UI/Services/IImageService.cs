namespace EmployeeManagement.UI.Services
{
    public interface IImageService
    {
        string GetImageUrl(string profilePath);
        string ConvertToRelativePath(string absolutePath);
    }
}
