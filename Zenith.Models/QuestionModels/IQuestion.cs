namespace Zenith.Models.Account;

public interface IQuestion
{
    
    string QuestionText { get; set; }
    Fraction Answer { get; set; }
    
    Dictionary<int, Func<string>> _generators { get; set; } //this will be the dictionary mapping the difficulty type to the method that will be used to generate the question
    
    public void Generate(int difficulty);
    
    public string GenerateHard();
    public string GenerateMedium();
    public string GenerateEasy();
    
    public bool CheckAnswer(string[] answer);

}