using System.Diagnostics;
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
        AnsweredQuestionStack answeredQuestions = new AnsweredQuestionStack();
        QuestioningSequence(questions , answeredQuestions);

    }

    private QuestionStack InitialiseStack() //used to generate a stack of questions 
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

    private AnsweredQuestionStack QuestioningSequence(QuestionStack questions, AnsweredQuestionStack answeredQuestions)
    {
        if(questions.isEmpty()) //if the user has answered all questions then stop the recursion
        {
            return answeredQuestions;
        }
        
        AdditionQuestion question = (questions.Pop() as AdditionQuestion)!;
        Stopwatch timer = new Stopwatch();
        QuestionText = question.QuestionText;
        timer.Start();
        
        
        
        
        
        return QuestioningSequence(questions , answeredQuestions);



    }
    
    
    
    
    
}