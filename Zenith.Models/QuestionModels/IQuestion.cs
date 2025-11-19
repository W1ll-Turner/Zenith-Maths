using Zenith.Models.QuestionModels;

namespace Zenith.Models.Account;

public interface IQuestion
{
    
    string QuestionText { get; set; }
    Fraction Answer { get; set; }
    string AnswerStringFormat { get; set; }
    Dictionary<int, Func<string>> Generators { get; set; } //this will be the dictionary mapping the difficulty type to the method that will be used to generate the question

    protected void Generate(int difficulty);
    
    public string GenerateHard();
    public string GenerateMedium();
    public string GenerateEasy();

    public bool CheckAnswer(Fraction userAnswer);

}