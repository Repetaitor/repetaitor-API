using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Essays;

namespace Infrastructure.ProjectServices.Implementations;

public class EssayService(IEssayRepository essayRepository) : IEssayService
{
    public async Task<ResponseView<EssayModal>> CreateNewEssay(string essayTitle, string essayDescription,
        int expectedWordCount, int creatorId)
    {
        return await essayRepository.CreateNewEssay(essayTitle, essayDescription, expectedWordCount, creatorId);
    }

    public async Task<ResponseView<ResultResponse>> DeleteEssay(int essayId, int byUser)
    {
        var res = await essayRepository.DeleteEssay(essayId, byUser);
        return new ResponseView<ResultResponse>
        {
            Code = res.Code,
            Message = res.Message,
            Data = new ResultResponse
            {
                Result = res.Data
            }
        };
    }

    public async Task<ResponseView<EssayModal>> UpdateEssay(int essayId, string essayTitle,
        string essayDescription,
        int expectedWordCount, int byUser)
    {
        return await essayRepository.UpdateEssay(essayId, essayTitle, essayDescription, expectedWordCount, byUser);
    }

    public async Task<ResponseView<List<EssayModal>>> GetUserEssays(int userId)
    {
        return await essayRepository.GetUserEssays(userId);
    }

    public async Task<ResponseView<EssayModal>> GetEssayById(int essayId)
    {
        return await essayRepository.GetEssay(essayId);
    }
}