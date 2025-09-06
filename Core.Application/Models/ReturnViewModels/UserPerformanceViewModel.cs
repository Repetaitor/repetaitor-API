namespace Core.Application.Models.ReturnViewModels;

public class UserPerformanceViewModel
{
    public List<PerformanceStat> PerformanceStats { get; set; } = [];
}
public class PerformanceStat
{
    public DateTime DateTime { get; set; }
    public double TotalScoreAvg { get; set; }
    public double GrammarScoreAvg { get; set; }
    public double FluencyScoreAvg { get; set; }
}