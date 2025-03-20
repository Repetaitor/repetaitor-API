using Core.Application.Models;
using Core.Application.Models.DTO.Essays;

namespace Core.Application.Interfaces.Services;

public interface IEssayService
{
    Task<ResponseViewModel<ResultResponse>> CreateNewEssay(string essayTitle, string essayDescription, int expectedWordCount, int creatorId);
    Task<ResponseViewModel<ResultResponse>> DeleteEssay(int essayId, int byUser);
    Task<ResponseViewModel<ResultResponse>> UpdateEssay(int essayId, string essayTitle, string essayDescription, int expectedWordCount, int byUser);
    Task<ResponseViewModel<List<EssayModal>>> GetUserEssays(int userId);
}