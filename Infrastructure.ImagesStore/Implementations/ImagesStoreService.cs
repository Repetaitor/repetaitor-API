using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Core.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ImagesStore.Implementations;

public class ImagesStoreService : IImagesStoreService
{
    private readonly BlobContainerClient _containerClient;

    public ImagesStoreService(IConfiguration config)
    {
        var connectionString = config["AzureStorage:ConnectionString"];
        var containerName = config["AzureStorage:ContainerName"];
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }
    public async Task<string> UploadBase64ImageAsync(int userId, int assignmentId, string base64Image)
    {
        
        var mimeType = "image/png"; 
        var base64 = base64Image;

        if (base64Image.Contains(","))
        {
            var parts = base64Image.Split(',');
            var header = parts[0]; 
            base64 = parts[1];

            var match = Regex.Match(header, @"data:(?<type>.+?);base64");
            if (match.Success)
            {
                mimeType = match.Groups["type"].Value;
            }
        }

        var extension = mimeType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            "image/bmp" => ".bmp",
            _ => ".png"
        };
        var bytes = Convert.FromBase64String(base64);
        var fileName = $"{userId}/{assignmentId}/{Guid.NewGuid()}{extension}";
        var blobClient = _containerClient.GetBlobClient(fileName);

        using var stream = new MemoryStream(bytes);
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task<bool> ClearUserAssignmentImages(int userId, int assignmentId)
    {
        await foreach (var blob in _containerClient.GetBlobsAsync(prefix: $"{userId}/{assignmentId}/"))
        {
            var blobClient = _containerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteIfExistsAsync();
        }

        return true;
    }
}