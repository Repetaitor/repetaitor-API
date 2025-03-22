using Core.Application.Models;
using Core.Domain.Entities;

namespace Core.Domain.Mappers;

public static class EssayMapper
{
    public static EssayModal ToEssayModal(Essay essay)
    {
        return new EssayModal()
        {
            Id = essay.Id,
            EssayTitle = essay.EssayTitle,
            EssayDescription = essay.EssayDescription,
            ExpectedWordCount = essay.ExpectedWordCount,
            CreatorId = essay.CreatorId,
            CreateDate = essay.CreateDate
        };
    }
}