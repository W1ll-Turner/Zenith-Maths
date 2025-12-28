namespace Zenith.Application.StatsCalculation;

public interface IStatsCalculation
{
    Task<double> CalculateTopicCompletion(int difficulty, int RoundsCompleted, double AverageTime , double AverageScore);
    Task<double> CalclulateOverallCompletion(double [] values);
    Task<int> GetBestTopicID(double[] values);
    Task<int> GetWorstTopicID(double[] values);
}