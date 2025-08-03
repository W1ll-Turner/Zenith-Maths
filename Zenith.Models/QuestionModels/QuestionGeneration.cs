namespace Zenith.Models.Account;

public class Addition : IQuestion
{
    public string QuestionText { get; set; }
    public string Answer { get; set; }
    public string Difficulty { get; set; }
    public Dictionary<string, Func<string>> _generators { get; set; }



    public Addition(string difficulty)
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
        if (_generators.TryGetValue(Difficulty,
                out Func<string> generator)) //if the key is within the dictionary it will call the coressponding function as the method generator()
        {
            QuestionText = generator();
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
        Fraction operand1 = new Fraction();

        throw new NotImplementedException();
    }

    public string GenerateEasy()
    {
        Random random = new Random();

        int operand1 = random.Next(0, 150);
        int operand2 = random.Next(0, 150);

        Answer = Convert.ToString(operand1 + operand2);
        return operand2.ToString() + operand1 + "=";
    }

    enum DecimalsForMedium
    {
        
    }
}
    

   

    
    
    
