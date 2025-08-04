namespace Zenith.Models.Account;

public interface IQuestion
{
    
    string QuestionText { get; set; }
    Fraction Answer { get; set; }
    string Difficulty {get;set;}
    
    Dictionary<string, Func<string>> _generators { get; set; } //this will be the dictionary mapping the difficulty type to the method that will be used to generate the question
    
    public void Generate();
    
    public string GenerateHard();
    public string GenerateMedium();
    public string GenerateEasy();

}