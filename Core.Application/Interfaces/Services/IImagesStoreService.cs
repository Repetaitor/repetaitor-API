using Core.Application.Models;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Interfaces.Services;

public interface IImagesStoreService
{
    // public Task<bool> StoreImagesAsync(int userId, int assignmentId, List<String> images);
    // public bool CreateDirectoryForUser(int userId);
    // public List<string> GetUserAssignmentImages(int userId, int assignmentId);
    Task<string> UploadBase64ImageAsync(int userId, int assignmentId, string base64Image);
    Task<bool> ClearUserAssignmentImages(int userId, int assignmentId);
}