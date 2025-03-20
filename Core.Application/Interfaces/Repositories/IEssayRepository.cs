using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IEssayRepository
{
    Task<bool> CreateNewEssay(string essayTitle, string essayDescription, int expectedWordCount, int creatorId);
    Task<bool> DeleteEssay(int essayId, int byUser);
    Task<bool> UpdateEssay(int essayId, string essayTitle, string essayDescription, int expectedWordCount, int byUser);
    Task<List<EssayModal>> GetUserEssays(int userId);
}