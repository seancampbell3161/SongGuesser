using Amazon.S3;
using Amazon.S3.Model;
using API.Interfaces;

namespace API.Services;

public class FileService(
    IAmazonS3 s3,
    IConfiguration configuration) : IFileService
{
    public async Task<List<string>> GetFilesAsync()
    {
        var response = await s3.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = configuration["SPACES_BUCKET_NAME"]
        });

        var keys = response.S3Objects.Select(o => o.Key).ToList();
        return keys;
    }
    
    public async Task SaveFilesAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var putReq = new PutObjectRequest
        {
            BucketName  = configuration["SPACES_BUCKET_NAME"],
            Key         = $"uploads/{file.FileName}",
            InputStream = stream,
            ContentType = file.ContentType
        };
        
        await s3.PutObjectAsync(putReq);
    }
}