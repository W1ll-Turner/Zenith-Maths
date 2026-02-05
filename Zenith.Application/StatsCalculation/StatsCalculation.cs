using Zenith.Models.Account;

namespace Zenith.Application.StatsCalculation;

public class StatsCalculation : IStatsCalculation
{
    public async Task<TopicAverages> CalculateTopicAverages(List<shorttermsstatsinfo> info)
    {
        //if there is nothing in the list will return an empty list of averages, so the averages arent calculautled
        if (info.Count == 0)
        {
            TopicAverages emptyTopic = new TopicAverages()
            {
                averageScore = 0.0,
                averageTime = 0.0,
                difficulty = 0,
                numberOfRounds = 0,
            };
            return emptyTopic;
        }
        
        //calucltuin the averges using the arithmetic mean 
        int totalDifficulty = 0;
        double totalTime = 0;
        int Totalscore = 0;
        int numberOfRounds = 0;
        foreach (var item in info)
        {
            totalDifficulty += item.difficulty;
            totalTime += item.averagetime;
            Totalscore += item.score;
            numberOfRounds++;
        }
        
        //initialising the averges class and adding the value to it
        TopicAverages Averages = new TopicAverages()
        {
            averageTime = (double)totalTime/numberOfRounds,
            averageScore = (double)Totalscore/numberOfRounds,
            numberOfRounds = numberOfRounds,
            difficulty = (double)totalDifficulty/numberOfRounds,
        };
        return Averages;
    }

    public async Task<double> CalculateTopicCompletion(TopicAverages averages)
    {
        double difficulty = averages.difficulty;
        int RoundsCompleted = averages.numberOfRounds;
        double AverageScore = averages.averageScore;
        double AverageTime = averages.averageTime;
        
        //using the mathemtical functions to code the summary statistics to a value between 0 and 1 
        double AverageScoreSigmoidValue = StatisticalFunctions.SigmoidAverageScore(AverageScore);
        double AverageTimeSigmoidValue = StatisticalFunctions.SigmoidAverageTime(AverageTime);
        double CodedDifficulty = (double)difficulty / 3.0;
        double CodedRoundsCompleted = (double)RoundsCompleted / 10.0;
        
        //finsding the geometric mean of those values
        double[] Values = new double[]{AverageScoreSigmoidValue, AverageTimeSigmoidValue, CodedDifficulty, CodedRoundsCompleted};
        double CompletionScore = StatisticalFunctions.GeometricMean(Values) ;

        //returng the completions 
        return CompletionScore;
    }
    public async Task<double> CalclulateOverallCompletion(Dictionary<int, double> completion)
    {
        //putting the values form the dictionary into an array so it can be passed to the geometric mean function
        double[] values = new double[completion.Count];
        int counter = 0;
        foreach (int Key in completion.Keys)
        {
            values[counter]  = completion[Key];
            counter++;
        }
        
        double CompletionScore = StatisticalFunctions.GeometricMean(values);
        return CompletionScore;
    }

    public async Task<double> CompoundCompletion(double currentCompletion, double newCompletion)
    {
        //meaning there is no current completion score and so simply return the one that has just been calculated
        if (double.IsNaN(currentCompletion))
        {
            return newCompletion;
        }
        //calculating the new completio sore 
        double temp = currentCompletion * newCompletion * 1.2;
        double completion = Math.Sqrt(temp);
              
        //making sure the score cannot exceed 1
        completion = Math.Min(1.0, completion);
        return completion;
    }

    public async Task<int> GetBestTopicID(Dictionary<int, double> completion)
    {
        double largestValue = 0;
        int largestValueKey = 0;
            
        foreach (int Key in completion.Keys)
        {
            if (completion[Key] > largestValue)
            {
                largestValue = completion[Key];
                largestValueKey = Key;;
            }
        }
        return largestValueKey;
    }
    public async Task<int> GetWorstTopicID(Dictionary<int, double> completion)
    {
        double lowestValue = 2.0;
        int lowestValueKey = 0;

        foreach (int Key in completion.Keys)
        {
            if (completion[Key] < lowestValue)
            {
                lowestValue = completion[Key];
                lowestValueKey = Key;
            }
        }
        
        return lowestValueKey;
    }

    public async Task<double[]> CalculateOverallAverageTimeAndScore(Dictionary<int, TopicAverages> averages)
    {
        double TotalScore = 0;
        double TotalTime = 0;
        double TotalDifficulty = 0;
        int counter = 0;
        
        foreach (int Key  in averages.Keys)
        {
            TotalScore += averages[Key].averageScore;
            TotalTime += averages[Key].averageTime;
            TotalDifficulty += averages[Key].difficulty;
            counter++;
        }

        double[] values = [(double)TotalScore/counter, (double)TotalTime/counter,(double)TotalDifficulty/counter];
        return values;
    }
}