using System.Text;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ImagesStoreService;

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

    // public async Task<bool> StoreImagesAsync(int userId, int assignmentId, List<String> images)
    // {
    //     try
    //     {
    //         var userDirectory = Directory.GetCurrentDirectory() + $"/ImagesStore/{userId}";
    //         if (!Directory.Exists(userDirectory))
    //         {
    //             var res = CreateDirectoryForUser(userId);
    //             if (res == false)
    //             {
    //                 throw new Exception("Failed to create user directory");
    //             }
    //         }
    //
    //         var assignmentDirectory = $"{userDirectory}/{assignmentId}";
    //         if (Directory.Exists(assignmentDirectory))
    //         {
    //             Directory.Delete(assignmentDirectory, recursive: true);
    //         }
    //         Directory.CreateDirectory(assignmentDirectory);
    //         for(var i = 0; i < images.Count; i++)
    //         {
    //             if (images[i].Length <= 0) continue;
    //             var filePath = Path.Combine(assignmentDirectory, $"image_{i}.txt");
    //             await using var stream = new FileStream(filePath, FileMode.Create);
    //             var imageBytes = Encoding.UTF8.GetBytes(images[i]);
    //             await stream.WriteAsync(imageBytes);
    //         }
    //
    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception("Error storing images: " + ex.Message);
    //     }
    // }
    //
    // public bool CreateDirectoryForUser(int userId)
    // {
    //     var baseImagesDirectory = Directory.GetCurrentDirectory() + "/ImagesStore";
    //     var directoryInfo = new DirectoryInfo(baseImagesDirectory);
    //     directoryInfo.CreateSubdirectory(userId.ToString());
    //     return true;
    // }
    //
    // public List<string> GetUserAssignmentImages(int userId, int assignmentId)
    // {
    //     var expectedCatalog = Directory.GetCurrentDirectory() + $"/ImagesStore/{userId}/{assignmentId}";
    //     if (!Directory.Exists(expectedCatalog))
    //     {
    //         Directory.CreateDirectory(expectedCatalog);
    //         return [];
    //     }
    //
    //     var directoryInfo = new DirectoryInfo(expectedCatalog);
    //     var imagesFiles = directoryInfo.GetFiles();
    //     var imagesBase64 = new List<string>();
    //     foreach (var file in imagesFiles)
    //     {
    //         if (file.Length <= 0) continue;
    //         var imageBytes = File.ReadAllBytes(file.FullName);
    //         var base64Image = System.Text.Encoding.UTF8.GetString(imageBytes, 0, imageBytes.Length);
    //         imagesBase64.Add(base64Image!);
    //     }
    //     return imagesBase64;
    // }
}