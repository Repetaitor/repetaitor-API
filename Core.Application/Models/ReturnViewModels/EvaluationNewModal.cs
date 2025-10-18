using Core.Application.Models.QuizModels;

namespace Core.Application.Models.ReturnViewModels;

public class EvaluationNewModal
{
    public EvaluationScore evaluationScore { get; set; }
    public QuizViewModel? quiz { get; set; }
}

public class EvaluationScore
{
    public int FluencyScore { get; set; }
    public int GrammarScore { get; set; }
    public List<EvaluationTextCommentModal> EvaluationTextComments { get; set; } = [];
    public List<GeneralCommentModal> GeneralComments { get; set; } = [];
}
