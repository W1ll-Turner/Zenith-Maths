using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;

namespace ZenithFrontEnd.Components.Pages.Questions;

public partial class Addition : ComponentBase
{
    public int Difficulty { get; set; }
    protected override async Task OnInitializedAsync()
    {
        QuestionStack questions = InitialiseStack();
            

    }

    private QuestionStack InitialiseStack() //used to generate a stack of questions 
    {
        QuestionStack questions = new QuestionStack();
        for (int i = 0; i < 10; i++)
        {
            AdditionQuestion question = new AdditionQuestion(Difficulty);
            questions.Push(question);

        }
        
        return questions;
    }
    
    
    
    
    
    
}