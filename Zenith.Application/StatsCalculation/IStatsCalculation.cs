using Zenith.Models.Account;

namespace Zenith.Application.StatsCalculation;

public interface IStatsCalculation
{
    Task<TopicAverages> CalculateTopicAverages(List<shorttermsstatsinfo> info);
    Task<double> CalculateTopicCompletion(TopicAverages average);
    Task<double> CalclulateOverallCompletion(Dictionary<int, double> completion);
    Task<int> GetBestTopicID(Dictionary<int, double> completion);
    Task<int> GetWorstTopicID(Dictionary<int, double> completion);

    Task<double[]> CalculateOverallAverageTimeAndScore(Dictionary<int, TopicAverages> averages);
}