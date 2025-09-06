using Core.Application.Models;
using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Interfaces.Repositories;

public interface IEssayRepository
{
    Task<EssayModal> CreateNewEssay(string essayTitle, string essayDescription, int expectedWordCount, int creatorId);
    Task<bool> DeleteEssay(int essayId, int byUser);

    Task<EssayModal> UpdateEssay(int essayId, string essayTitle, string essayDescription, int expectedWordCount,
        int byUser);

    Task<List<EssayModal>> GetUserEssays(int userId);
    Task<EssayModal> GetEssay(int essayId);
    Task<int> GetEssayCount(int userId);
    Task<bool> EssayExists(int essayId);
    Task<bool> IsUserEssayCreator(int userId, int essayId);
}