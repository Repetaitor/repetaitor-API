using Core.Application.Models;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Interfaces.Services;

public interface IImagesStoreService
{
    public Task<bool> StoreImagesAsync(int userId, int assignmentId, List<String> images);
    public bool CreateDirectoryForUser(int userId);
    public FileInfo[] GetUserAssignmentImages(int userId, int assignmentId);
}