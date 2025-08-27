using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;

namespace ZenithFrontEnd.Components.Pages.Questions.Topic_Components;

public partial class Addition : ComponentBase
{
    public int Difficulty { get; set; }
    public string QuestionText { get; set; } = "";
    public string UserAnswer { get; set; } = "";
    public QuestionStack QuestionStack { get; set; } = new QuestionStack();
    public bool Error = false;
    protected override async Task OnInitializedAsync()
    {
        Error = false;
        QuestionStack = InitialiseStack(2  ); //initialising the questions // gett the actruial difficulty from the local storgae
        AnsweredQuestionStack answeredQuestions = new AnsweredQuestionStack();
        

    }

    private QuestionStack InitialiseStack(int difficulty) //used to generate a stack of questions 
    {
        int testdifficulty = 1;
        
        QuestionStack questions = new QuestionStack();
        for (int i = 0; i < 10; i++)
        {
            AdditionQuestion question = new AdditionQuestion(testdifficulty);
            Console.WriteLine("pushing item to the stack");
            questions.Push(question);

        }
        
        return questions;
    }

    private void AnswerQuesation()
    {
        string answerPattern = "^[0-9](\\/[0-9])?$";
        Regex answerRG = new Regex(answerPattern);
        if (answerRG.IsMatch(QuestionText))
        {
            string[] answer = UserAnswer.Split("/");
            
        }
        {
            Error = true;
        }
        


    }
    
    
    
    
}