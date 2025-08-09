using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;

namespace ZenithFrontEnd.Components.Pages.Questions.Topic_Components;

public partial class Addition : ComponentBase
{
    public int Difficulty { get; set; }
    public string QuestionText { get; set; } = "";
    public string UserAnswer { get; set; } = "";
    protected override async Task OnInitializedAsync()
    {
        QuestionStack questions = InitialiseStack(); //initialising the questions
        QuestioningSequence(questions);

    }

    private QuestionStack InitialiseStack() //used to generate a stack of questions 
    {
        int testdifficulty = 1;
        
        QuestionStack questions = new QuestionStack();
        for (int i = 0; i < 10; i++)
        {
            AdditionQuestion question = new AdditionQuestion(testdifficulty);
            questions.Push(question);

        }
        
        return questions;
    }

    public void QuestioningSequence(QuestionStack questions)
    {
        AdditionQuestion question = (questions.Pop() as AdditionQuestion)!;
        QuestionText = question.QuestionText;
        
        

    }
    
    
    
    
    
}