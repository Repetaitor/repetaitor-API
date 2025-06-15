using Core.Application.Interfaces.Services;

namespace TestProject;

public class FakeImageStoreService : IImagesStoreService
{
    public Task<string> UploadBase64ImageAsync(int userId, int assignmentId, string base64Image)
    {
        return Task.FromResult("test_image_url");
    }

    public Task<bool> ClearUserAssignmentImages(int userId, int assignmentId)
    {
        return Task.FromResult(true);
    }
}