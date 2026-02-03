namespace Zenith.Application.Hashing;

//creating the interface for how the hashing class should be implemented
public interface IHashing
{
    public Task<string> GenerateShortTermStatsId(string studentId);
    
    public Task<string> GenerateLongTermStatsId(string studentId);
}