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
    private IQuestion CurrentQuestion { get; set; }
    private QuestionModels.AnsweredQuestionStack AnsweredQuestionStack { get; set; } 
    public Dictionary<string, Func<bool>> TopicsMapper { get; set; }
    
    public bool StopQuestioning { get; set; } = false;

    private void Start()
    {
        StopQuestioning = true;
        Console.WriteLine("started");
        //this dictionary maps the topic to the genric methods that will generate the questions, This is dependant on a topic
        TopicsMapper = new Dictionary<string, Func<bool>>()
        {
            //the key is the topic and the vlue is a function call that will call the Intitlaise stack method using the appropiate question type 
            { "addition", InitialiseStack<AdditionQuestion> },
            { "subtraction", InitialiseStack<SubtractionQuestion> },
            { "multiplication", InitialiseStack<MultiplicationQuestion> },
            { "division", InitialiseStack<DivisionQuestion> },
            { "differentiation", InitialiseStack<DifferentiationQuestion> },
            { "integration", InitialiseStack<IntegrationQuestion> },
            { "quadratics", InitialiseStack<QuadraticsQuestion> },
            { "collectingterms", InitialiseStack<CollectingTermsQuestion> },
            { "everything", InitialiseStack<TestEverything> }

        };

        Console.WriteLine("dictionary has been made");
        //initilaising the question stack, if the topic cannot be found an exception will be thrown
        try
        {
            TopicsMapper.TryGetValue("addition", out Func<bool>? intialiseStack);
            Console.WriteLine("dictionary one worked initilaisng the stack");

            AnsweredQuestionStack = new QuestionModels.AnsweredQuestionStack();
            intialiseStack!();
            
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
            
        }
    }
    
    private bool InitialiseStack<T>() where T : IQuestion, new() //used to generate a stack of questions 
    {
        ;
        int testdifficulty = 1;
        QuestionModels.QuestionStack questions = new QuestionModels.QuestionStack();

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("Question made {0}", i+1);
            T question = new T();
            question.Difficulty = testdifficulty;
            question.Generate();
            questions.Push(question);
        }
 
        Questions = questions;
        QuestionSequence();
        return true;
    }
    
    private bool TestEverything()
    {
        return true;
    }
    
    private void QuestionSequence()
    {
        CurrentQuestion = Questions.Pop(); //getting the next question off the stack
       
        QuestionText = CurrentQuestion.QuestionText; //displaying the new question

        TimeToAnswer.Start(); //starting the timer 
    }
    
    public void AnswerQuestion()
    {
        Console.WriteLine("Questions answered");
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
        Console.WriteLine("Starting the if statement");
        if (Questions.IsEmpty()) 
        {

            Console.WriteLine("Questions stack is now empty");
            StopQuestioning = false;
            SendResultsToAPI(); //round of questioning has finshed time, send results to API then move user onto the summary screen 

        }
        else
        {
            Console.WriteLine("Starting the Question Sequence");   
            QuestionSequence();
        }
        
    }

    private void  SendResultsToAPI()
    {
      
        
        
        
    }
}