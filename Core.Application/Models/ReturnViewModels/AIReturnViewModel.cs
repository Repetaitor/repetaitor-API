namespace Core.Application.Models.ReturnViewModels;

public class AIReturnViewModel
{
    public int FluencyScore { get; set; }
    public int GrammarScore { get; set; }
    public string MarkedEssayText { get; set; } = "";
    public DetailedGrammar DetailedGrammarScore { get; set; } = default!;
    public List<GeneralCommentModal> GeneralComments { get; set; } = [];
}

public class DetailedGrammar
{
    public int Vocabulary { get; set; } 
    public int SpellingAndPunctuation { get; set; } 
    public int Grammar { get; set; } 
}