using Core.Application.Models;
using Core.Application.Models.RequestsDTO.Essays;
using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Interfaces.Services;

public interface IEssayService
{
    Task<ResponseView<EssayModal>> CreateNewEssay(string essayTitle, string essayDescription,
        int expectedWordCount, int creatorId);

    Task<ResponseView<ResultResponse>> DeleteEssay(int essayId, int byUser);

    Task<ResponseView<EssayModal>> UpdateEssay(int essayId, string essayTitle, string essayDescription,
        int expectedWordCount, int byUser);

    Task<ResponseView<List<EssayModal>>> GetUserEssays(int userId);
    Task<ResponseView<EssayModal>> GetEssayById(int userId);
}