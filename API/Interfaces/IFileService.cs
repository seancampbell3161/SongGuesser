namespace API.Interfaces;

public interface IFileService
{
    Task<List<string>> GetFilesAsync();
    Task SaveFilesAsync(IFormFile file);
}