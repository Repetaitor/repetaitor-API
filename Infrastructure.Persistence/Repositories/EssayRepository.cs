using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Infrastructure.Persistence.AppContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EssayRepository(ApplicationContext context) : IEssayRepository
{
    public async Task<EssayModal> CreateNewEssay(string essayTitle, string essayDescription, int expectedWordCount,
        int creatorId)
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
        return EssayMapper.ToEssayModal(newEssay);
    }

    public async Task<bool> DeleteEssay(int essayId, int byUser)
    {
        var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
        if (essay == null)
        {
            throw new Exception("Essay not found");
        }

        if (essay.CreatorId != byUser)
        {
            throw new Exception("You are not allowed to delete this essay");
        }

        context.Remove(essay);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<EssayModal> UpdateEssay(int essayId, string essayTitle, string essayDescription,
        int expectedWordCount, int byUser)
    {
        var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
        if (essay == null)
        {
            throw new Exception("Essay not found");
        }

        if (essay.CreatorId != byUser)
        {
            throw new Exception("You are not allowed to update this essay");
        }

        essay.EssayTitle = essayTitle;
        essay.EssayDescription = essayDescription;
        essay.ExpectedWordCount = expectedWordCount;
        await context.SaveChangesAsync();
        return EssayMapper.ToEssayModal(essay);
    }

    public async Task<List<EssayModal>> GetUserEssays(int userId)
    {
        return await context.Essays.Where(x => x.CreatorId == userId).Select(x => EssayMapper.ToEssayModal(x))
            .AsNoTracking().ToListAsync();
    }

    public async Task<EssayModal> GetEssay(int essayId)
    {
        var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
        if (essay == null)
        {
            throw new Exception("Essay not found");
        }

        return EssayMapper.ToEssayModal(essay);
    }

    public async Task<int> GetEssayCount(int userId)
    {
        return await context.Essays.CountAsync(x => x.CreatorId == userId);
    }

    public async Task<bool> EssayExists(int essayId)
    {
        return await context.Essays.AnyAsync(x => x.Id == essayId);
    }

    public async Task<bool> IsUserEssayCreator(int userId, int essayId)
    {
        return await context.Essays.AnyAsync(x => x.Id == essayId && x.CreatorId == userId);
    }
}