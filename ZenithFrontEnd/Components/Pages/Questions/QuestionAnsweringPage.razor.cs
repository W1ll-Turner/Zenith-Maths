using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Questions;


public partial class QuestionAnsweringPage:ComponentBase
 
{
    [Parameter]
    public int Difficulty { get; set; }
    
    [Parameter]
    public string Topic { get; set; } = null!;
    private string QuestionText { get; set; } = "";
    private string UserAnswer { get; set; } = "";
    private Stopwatch TimeToAnswer { get; set; } = new Stopwatch();
    
    private QuestionModels.QuestionStack Questions { get; set; } = new QuestionModels.QuestionStack();
    private AdditionQuestion CurrentQuestion { get; set; }
    private QuestionModels.AnsweredQuestionStack AnsweredQuestionStack { get; set; } 
    private void Start()
    {
        Questions = InitialiseStack(); //initialising the questions // gett the actruial difficulty from the route 
        AnsweredQuestionStack = new QuestionModels.AnsweredQuestionStack();
        Console.WriteLine(Questions.Pointer);
        QuestionSequence(); //prepare the question for the user to answer
    }




    private QuestionModels.QuestionStack InitialiseStack() //used to generate a stack of questions 
    {
        int testdifficulty = 1;
        QuestionModels.QuestionStack questions = new QuestionModels.QuestionStack();
        switch (Topic)
        {
            case "addition":
                for (int i = 0; i < 10; i++)
                {
                    AdditionQuestion question = new AdditionQuestion(testdifficulty);
                    Console.WriteLine("pushing item to the stack");
                    questions.Push(question);
                }

                break;
                /**
                case "subtraction":
                    for (int i = 0; i < 10; i++)
                    {
                        SubtractionQuestion question = new SubtractionQuestion(testdifficulty);
                        Console.WriteLine("pushing item to the stack");
                        questions.Push(question);
                    }
                    break;
                case "multiplication":
                    for (int i = 0; i < 10; i++)
                    {
                        MultiplicationQuestion question = new SubtractionQuestion(testdifficulty);
                        Console.WriteLine("pushing item to the stack");
                        questions.Push(question);
                    }
                    break;
                case "division":
                    for (int i = 0; i < 10; i++)
                    {
                        DivisionQuestion question = new DivisionQuestion(testdifficulty);
                        Console.WriteLine("pushing item to the stack");
                        questions.Push(question);
                    }
                    break;
                case "differentiation":
                    for (int i = 0; i < 10; i++)
                    {
                        DifferentiationQuestion question = new DifferentiationQuestion();
                        Console.WriteLine("pushing item to the stack");
                        questions.Push(question);
                    }
                    break;
                case "integration":
                    for (int i = 0; i < 10; i++)
                    {
                        IntegrationQuestion question = new Integrationuestion(testdifficulty);
                        Console.WriteLine("pushing item to the stack");
                        questions.Push(question);
                    }
                    break;
                case "quadratics":
                    for (int i = 0; i < 10; i++)
                    {
                        QuadraticsQuestion question = new QuadraticsQuestion(testdifficulty);
                        Console.WriteLine("pushing item to the stack");
                        questions.Push(question);
                    }
                    break;
                case "collectingterms":
                    for (int i = 0; i < 10; i++)
                    {
                        CollectingTermsQuestion question = new CollectingTermsQuestion(testdifficulty);
                        Console.WriteLine("pushing item to the stack");
                        questions.Push(question);
                    }
                    break;
                case "everything:":
                    //this will requre some specialised code for the code
                    break;

            }
            **/ 
                
        }
        return questions;
    }


    private void QuestionSequence()
    {
        
   
        CurrentQuestion = (AdditionQuestion)Questions.Pop(); //getting the next question off the stack \
       
        QuestionText = CurrentQuestion.QuestionText; //displaying the new question

        TimeToAnswer.Start(); //starting the timer 
     
        
    }
    

    public void AnswerQuestion()
    {
        
        TimeToAnswer.Stop(); //stopping the timer 
        bool AnswerCorrect = false;
        
        string answerPattern = "^[0-9](\\/[0-9])?$";
        Regex answerRG = new Regex(answerPattern); //this will make sure the answer has been given withing the correct format
        
        
        if (answerRG.IsMatch(UserAnswer)) //checking the answer is appropraite to start checking
        {
            string[] answer = UserAnswer.Split("/"); //if the answer is a fraction it will be 
            if (answer.Length == 1)
            {
                Fraction input = new Fraction(int.Parse(answer[0]), 1);
                AnswerCorrect = CurrentQuestion.CheckAnswer(input);
            }else if (answer.Length == 2)
            {
                Fraction input = new Fraction(int.Parse(answer[0]), int.Parse(answer[1]));
                AnswerCorrect = CurrentQuestion.CheckAnswer(input);
            }
        }
        
        QuestionModels.AnsweredQuestion answeredQuestion = new QuestionModels.AnsweredQuestion(AnswerCorrect , CurrentQuestion.AnswerStringFormat , UserAnswer , CurrentQuestion.QuestionText ,TimeToAnswer.ElapsedMilliseconds);
        AnsweredQuestionStack.Push(answeredQuestion);
        Console.WriteLine(Questions.Pointer);
        if (Questions.IsEmpty())
        {
            NavigationManager.NavigateTo("/Home");
            SendResultsToAPI(); //round of questioning has finshed time, send results to API then move user onto the summary screen 

        }
        else
        {
            QuestionSequence();
        }
        
        


    }

    private void  SendResultsToAPI()
    {
        
    }
}