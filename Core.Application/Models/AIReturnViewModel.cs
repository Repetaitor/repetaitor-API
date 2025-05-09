using Core.Application.Models;
using Core.Application.Models.DTO;

namespace Core.Application.Models;

public class AIReturnViewModel
{
    public int FluencyScore { get; set; }
    public int GrammarScore { get; set; }
    public List<EvaluationTextCommentModal> EvaluationTextComments { get; set; } = [];
    public List<GeneralCommentModal> GeneralComments { get; set; } = [];
}