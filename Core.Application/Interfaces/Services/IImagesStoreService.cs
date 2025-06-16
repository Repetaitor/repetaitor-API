using Core.Application.Models;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Interfaces.Services;

public interface IImagesStoreService
{
    Task<string> UploadBase64ImageAsync(int userId, int assignmentId, string base64Image);
    Task<bool> ClearUserAssignmentImages(int userId, int assignmentId);
}