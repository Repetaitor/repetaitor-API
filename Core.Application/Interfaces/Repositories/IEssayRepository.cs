using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IEssayRepository
{
    Task<ResponseView<EssayModal>> CreateNewEssay(string essayTitle, string essayDescription, int expectedWordCount, int creatorId);
    Task<ResponseView<bool>> DeleteEssay(int essayId, int byUser);

    Task<ResponseView<EssayModal>> UpdateEssay(int essayId, string essayTitle, string essayDescription, int expectedWordCount,
        int byUser);

    Task<ResponseView<List<EssayModal>>> GetUserEssays(int userId);
    Task<ResponseView<EssayModal>> GetEssay(int essayId);
}