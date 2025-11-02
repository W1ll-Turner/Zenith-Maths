namespace Zenith.Application.Hashing;

public interface IHashing
{
    public Task<string> GenerateShortTermStatsID(string StudentId);
    
    public Task<string> GenerateLongTermStatsID(string StudentId);
}