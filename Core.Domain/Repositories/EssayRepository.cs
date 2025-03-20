using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Repositories;

public class EssayRepository(ApplicationContext context) : IEssayRepository
{
    public async Task<bool> CreateNewEssay(string essayTitle, string essayDescription, int expectedWordCount, int creatorId)
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
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteEssay(int essayId, int byUser)
    {
        try
        {
            var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
            if (essay == null || essay.CreatorId != byUser) return false;
            context.Remove(essay);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateEssay(int essayId, string essayTitle, string essayDescription, int expectedWordCount, int byUser)
    {
        try
        {
            var essay = await context.Essays.FirstOrDefaultAsync(x => x.Id == essayId);
            if (essay == null || essay.CreatorId != byUser) return false;
            essay.EssayTitle = essayTitle;
            essay.EssayDescription = essayDescription;
            essay.ExpectedWordCount = expectedWordCount;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<EssayModal>> GetUserEssays(int userId)
    {
        try
        {
            var essay = await context.Essays.Where(x => x.CreatorId == userId).Select(x => new EssayModal
            {
                Id = x.Id,
                CreatorId = x.CreatorId,
                EssayTitle = x.EssayTitle,
                EssayDescription = x.EssayDescription,
                ExpectedWordCount = x.ExpectedWordCount
            }).AsNoTracking().ToListAsync();
            return essay;
        }
        catch (Exception)
        {
            return [];
        }
    }
}