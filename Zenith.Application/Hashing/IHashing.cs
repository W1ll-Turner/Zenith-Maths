namespace Zenith.Application.Hashing;

public interface IHashing
{
    public Task<string> GenerateShortTermStatsID(string StudentId);

    public Task<string> GenerateRoundID(string StudentID);
    
    public Task<string> GenerateLongTermStatsID(string StudentId);
}