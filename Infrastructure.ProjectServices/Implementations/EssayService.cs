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
        try
        {
            var res = await essayRepository.CreateNewEssay(essayTitle, essayDescription, expectedWordCount, creatorId);
            return new ResponseView<EssayModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<EssayModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> DeleteEssay(int essayId, int byUser)
    {
        try
        {
            var res = await essayRepository.DeleteEssay(essayId, byUser);
            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.Success,
                Data = new ResultResponse
                {
                    Result = res
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<EssayModal>> UpdateEssay(int essayId, string essayTitle,
        string essayDescription,
        int expectedWordCount, int byUser)
    {
        try
        {
            var res = await essayRepository.UpdateEssay(essayId, essayTitle, essayDescription, expectedWordCount,
                byUser);
            return new ResponseView<EssayModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<EssayModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<List<EssayModal>>> GetUserEssays(int userId)
    {
        try
        {
            var res =  await essayRepository.GetUserEssays(userId);
            return new ResponseView<List<EssayModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<EssayModal>>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<EssayModal>> GetEssayById(int essayId)
    {
        try
        {
            var res = await essayRepository.GetEssay(essayId);
            return new ResponseView<EssayModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        } catch (Exception ex)
        {
            return new ResponseView<EssayModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}