namespace Core.Application.Models.ReturnViewModels;

public class AIReturnViewModel
{
    public int FluencyScore { get; set; }
    public int GrammarScore { get; set; }
    public string MarkedEssayText { get; set; } = "";
    public List<GeneralCommentModal> GeneralComments { get; set; } = [];
}