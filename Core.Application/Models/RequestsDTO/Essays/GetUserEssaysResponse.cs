using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Models.RequestsDTO.Essays;

public class GetUserEssaysResponse
{
    public List<EssayModal> Essays { get; set; } = [];
}