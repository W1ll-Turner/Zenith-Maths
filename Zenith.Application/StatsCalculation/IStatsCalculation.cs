using Zenith.Models.Account;

namespace Zenith.Application.StatsCalculation;

public interface IStatsCalculation
{
    Task<TopicAverages> CalculateTopicAverages(List<shorttermsstatsinfo> info);
    Task<double> CalculateTopicCompletion(TopicAverages average);
    Task<double> CalclulateOverallCompletion(double [] values);
    Task<int> GetBestTopicID(double[] values);
    Task<int> GetWorstTopicID(double[] values);

    Task<double[]> CalculateAverageTimeAndScore(TopicAverages[] averages);
}