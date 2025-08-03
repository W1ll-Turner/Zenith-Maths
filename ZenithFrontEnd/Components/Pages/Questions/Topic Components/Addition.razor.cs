using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;

namespace ZenithFrontEnd.Components.Pages.Questions;

public partial class Addition : ComponentBase
{
    class AdditionQuestion : IQuestion //Inheriting from the base Question interface 
    {
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public string Difficulty { get; set; }
        public Dictionary<string, Func<string>> _generators { get; set; }

        

        public AdditionQuestion(string difficulty)
        {
            Difficulty = difficulty.ToString();
            QuestionText = difficulty;

            _generators = new Dictionary<string, Func<string>>
            {
                { "easy", GenerateEasy },
                { "medium", GenerateMedium },
                { "hard", GenerateHard }
            };
        }
        public void Generate()
        {
            if (_generators.TryGetValue(Difficulty, out Func<string> generator)) //if the key is within the dictionary it will call the coressponding function as the method generator()
            {
                AnswerText = generator();
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public string GenerateHard()
        {
            throw new NotImplementedException();
        }

        public string GenerateMedium()
        {
            throw new NotImplementedException();
        }

        

        public void Evaluate()
        {
            throw new NotImplementedException();
        }

        
    }
    
    
    
    
}