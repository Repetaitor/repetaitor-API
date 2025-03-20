using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Essays;

namespace Infrastructure.ProjectServices.Implementations;

public class EssayService(IEssayRepository essayRepository) : IEssayService
{
    public async Task<ResponseViewModel<ResultResponse>> CreateNewEssay(string essayTitle, string essayDescription,
        int expectedWordCount, int creatorId)
    {
        var res = await essayRepository.CreateNewEssay(essayTitle, essayDescription, expectedWordCount, creatorId);

        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : 1,
            Message = res ? "Essay added" : "Something went wrong",
            Data = new ResultResponse()
            {
                Result = res
            }
        };
    }

    public async Task<ResponseViewModel<ResultResponse>> DeleteEssay(int essayId, int byUser)
    {
        var res = await essayRepository.DeleteEssay(essayId, byUser);
        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : 1,
            Message = res ? "Essay removed" : "Something went wrong",
            Data = new ResultResponse()
            {
                Result = res
            }
        };
    }

    public async Task<ResponseViewModel<ResultResponse>> UpdateEssay(int essayId, string essayTitle, string essayDescription,
        int expectedWordCount, int byUser)
    {
        var res = await essayRepository.UpdateEssay(essayId, essayTitle, essayDescription, expectedWordCount, byUser);
        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : 1,
            Message = res ? "Essay Updated" : "Something went wrong",
            Data = new ResultResponse()
            {
                Result = res
            }
        };
    }

    public async Task<ResponseViewModel<List<EssayModal>>> GetUserEssays(int userId)
    {
        var res = await essayRepository.GetUserEssays(userId);
        return new ResponseViewModel<List<EssayModal>>()
        {
            Code = 0,
            Message = "",
            Data = res
        };
    }
}