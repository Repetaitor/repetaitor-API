using Core.Application.Models;
using Core.Application.Models.DTO.Essays;

namespace Core.Application.Interfaces.Services;

public interface IEssayService
{
    Task<EssayModal?> CreateNewEssay(string essayTitle, string essayDescription,
        int expectedWordCount, int creatorId);

    Task<ResultResponse?> DeleteEssay(int essayId, int byUser);

    Task<EssayModal?> UpdateEssay(int essayId, string essayTitle, string essayDescription,
        int expectedWordCount, int byUser);

    Task<List<EssayModal>?> GetUserEssays(int userId);
    Task<EssayModal?> GetEssayById(int userId);
}