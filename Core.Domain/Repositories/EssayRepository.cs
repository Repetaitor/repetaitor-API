using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Repositories;

public class EssayRepository(ApplicationContext context) : IEssayRepository
{
    public async Task<ResponseView<EssayModal>> CreateNewEssay(string essayTitle, string essayDescription, int expectedWordCount,
        int creatorId)
    {
        try
        {
            var newEssay = new Essay
            {
                EssayTitle = essayTitle,
                EssayDescription = essayDescription,
                ExpectedWordCount = expectedWordCount,
                CreatorId = creatorId
            };
            await context.Essays.AddAsync(newEssay);
            await context.SaveChangesAsync();
            return new ResponseView<EssayModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = EssayMapper.ToEssayModal(newEssay)
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

    public async Task<ResponseView<bool>> DeleteEssay(int essayId, int byUser)
    {
        try
        {
            var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
            if (essay == null)
            {
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Essay not found",
                    Data = false
                };
            }
            if(essay.CreatorId != byUser)
            {
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not allowed to delete this essay",
                    Data = false
                };
            }
            context.Remove(essay);
            await context.SaveChangesAsync();
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.Success,
                Message = "Essay deleted successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<EssayModal>> UpdateEssay(int essayId, string essayTitle, string essayDescription,
        int expectedWordCount, int byUser)
    {
        try
        {
            var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
            if (essay == null)
            {
                return new ResponseView<EssayModal>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Essay not found",
                    Data = null
                };
            }
            if(essay.CreatorId != byUser)
            {
                return new ResponseView<EssayModal>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not allowed to delete this essay",
                    Data = null
                };
            }
            essay.EssayTitle = essayTitle;
            essay.EssayDescription = essayDescription;
            essay.ExpectedWordCount = expectedWordCount;
            await context.SaveChangesAsync();
            return new ResponseView<EssayModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = EssayMapper.ToEssayModal(essay)
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
            var essay = await context.Essays.Where(x => x.CreatorId == userId).Select(x => EssayMapper.ToEssayModal(x))
                .AsNoTracking().ToListAsync();
            return new ResponseView<List<EssayModal>>()
            {
                Code = StatusCodesEnum.Success,
                Data = essay
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

    public async Task<ResponseView<EssayModal>> GetEssay(int essayId)
    {
        try
        {
            var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
            if (essay == null)
            {
                return new ResponseView<EssayModal>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Essay not found",
                    Data = null
                };
            }
            return new ResponseView<EssayModal>
            {
                Code = StatusCodesEnum.Success,
                Data = EssayMapper.ToEssayModal(essay)
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
}