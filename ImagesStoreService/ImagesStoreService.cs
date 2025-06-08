using System.Text;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Microsoft.AspNetCore.Http;

namespace ImagesStoreService;

public class ImagesStoreService : IImagesStoreService
{
    public async Task<bool> StoreImagesAsync(int userId, int assignmentId, List<String> images)
    {
        try
        {
            var userDirectory = Directory.GetCurrentDirectory() + $"/ImagesStore/{userId}";
            if (!Directory.Exists(userDirectory))
            {
                var res = CreateDirectoryForUser(userId);
                if (res == false)
                {
                    throw new Exception("Failed to create user directory");
                }
            }

            var assignmentDirectory = $"{userDirectory}/{assignmentId}";
            if (Directory.Exists(assignmentDirectory))
            {
                Directory.Delete(assignmentDirectory, recursive: true);
            }
            Directory.CreateDirectory(assignmentDirectory);
            for(var i = 0; i < images.Count; i++)
            {
                if (images[i].Length <= 0) continue;
                var filePath = Path.Combine(assignmentDirectory, $"image_{i}.txt");
                await using var stream = new FileStream(filePath, FileMode.Create);
                var imageBytes = Encoding.UTF8.GetBytes(images[i]);
                await stream.WriteAsync(imageBytes);
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error storing images: " + ex.Message);
        }
    }

    public bool CreateDirectoryForUser(int userId)
    {
        var baseImagesDirectory = Directory.GetCurrentDirectory() + "/ImagesStore";
        var directoryInfo = new DirectoryInfo(baseImagesDirectory);
        directoryInfo.CreateSubdirectory(userId.ToString());
        return true;
    }

    public FileInfo[] GetUserAssignmentImages(int userId, int assignmentId)
    {
        var expectedCatalog = Directory.GetCurrentDirectory() + $"/ImagesStore/{userId}/{assignmentId}";
        if (!Directory.Exists(expectedCatalog))
            throw new DirectoryNotFoundException($"Directory not found: {expectedCatalog}");
        var directoryInfo = new DirectoryInfo(expectedCatalog);
        return directoryInfo.GetFiles();
    }
}