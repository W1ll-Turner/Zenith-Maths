using Zenith.Models.QuestionModels;

namespace Zenith.Models.Account;

public interface IQuestion<T>
{
    
    string QuestionText { get; set; }
    T Answer { get; set; }
    string AnswerStringFormat { get; set; }
    Dictionary<int, Func<string>> Generators { get; set; } //this will be the dictionary mapping the difficulty type to the method that will be used to generate the question
    
    int Difficulty { get; set; }
    public void Generate();
    
    public string GenerateHard();
    public string GenerateMedium();
    public string GenerateEasy();

    public bool CheckAnswer(T UserAnswer);

}

