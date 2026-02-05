using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Zenith.Contracts.Request.Account;
using Zenith.Models.Account;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Questions;

public partial class ComplexQuestionAnsweringPage : ComponentBase
{
    private int Difficulty { get; set; }
    [Parameter]
    public string Topic { get; set; } = null!;
    private string QuestionText { get; set; } = "";
    private string UserAnswer { get; set; } = "";
    private Stopwatch TimeToAnswer { get; set; } = new Stopwatch();
    private QuestionModels.QuestionStack<string> Questions { get; set; } = new QuestionModels.QuestionStack<string>();
    private IQuestion<string> CurrentQuestion { get; set; }
    private List<QuestionModels.AnsweredQuestion> AnsweredQuestions = new List<QuestionModels.AnsweredQuestion>();
    private Dictionary<string, Func<bool>> TopicsMapper { get; set; }
    private bool StopQuestioning { get; set; } = false;
    private int[] CorrectAnswers { get; set; } = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    private int questionNum = 0;
    private bool authenticated = false;
    private string StudentId;

    protected override async Task OnAfterRenderAsync(bool firstRender) //This is getting the user's ID from local storage, to make sure it is ready to be passed into the API calls
    {
        if (firstRender)
        {
            string Id = await GetId();
            Console.WriteLine(Id);
            if (Id == null)
            {
                authenticated = false;
            }
            else
            {
                authenticated = true;
            }
            StateHasChanged();
        }
    }
    
    private void Start(int difficulty)
    {
        Difficulty = difficulty;
        
        StopQuestioning = true;
        //this dictionary maps the topic to the genric methods that will generate the questions, This is dependant on a topic
        TopicsMapper = new Dictionary<string, Func<bool>>()
        {
            {"quadratic", InitialiseQuestionStack<QuadraticQuestion>},
        };
        
        //attempting to intilaise the stack of questions if the topic parameter is present in the route 
        try
        {
            TopicsMapper.TryGetValue(Topic, out Func<bool>? intialiseStack);
            intialiseStack!();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private bool InitialiseQuestionStack<TQuestion>() where TQuestion : IQuestion<string>, new()
    {
        QuestionModels.QuestionStack<string> questions = new QuestionModels.QuestionStack<string>();

        //genrating 10 quesitons and pushing them onto the stack
        for (int i = 0; i < 10; i++)
        {
            TQuestion question = new TQuestion();
            question.Difficulty = Difficulty;
            question.Generate();
            questions.Push(question);
        }
 
        Questions = questions;
        QuestionSequence();
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
        //stopping the timer 
        TimeToAnswer.Stop(); 
        
        //checking the answwer updating the UI elements to reflect that 
        bool answerCorrect = CurrentQuestion.CheckAnswer(UserAnswer);
        CorrectAnswers[questionNum] = answerCorrect? 1:2;
        questionNum++;
        StateHasChanged();
        NextQuestion(answerCorrect);
    }
    
    private void NextQuestion(bool AnswerCorrect)
    {
        //adding the answered question to the stack 
        QuestionModels.AnsweredQuestion answeredQuestion = new QuestionModels.AnsweredQuestion
        {
            Correct = AnswerCorrect,
            CorrectAnswer = CurrentQuestion.AnswerStringFormat,
            UserAnswer = UserAnswer,
            Question = CurrentQuestion.QuestionText,
            TimeTaken = TimeToAnswer.ElapsedMilliseconds/1000.0 //converting the milliseconds into seconds 
        };
        AnsweredQuestions.Add(answeredQuestion);
        TimeToAnswer.Reset(); //reseting stopwatch 
        
        //checkign whether to stop or carrying asking questions
        if (Questions.IsEmpty()) 
        {
            StopQuestioning = false;
            SendResultsToAPI(); //round of questioning has finshed time, send results to API then move user onto the summary screen 
        }
        else
        {
            QuestionSequence();
        }
    }
    
    private async Task  SendResultsToAPI()
    {
        //resetting the array keeping track of the user's answers 
        ResetArray();
        
        //getting the current time 
        DateTime temporaryTimeHolder = DateTime.Now;
        string time = temporaryTimeHolder.ToString("HH:mm:ss");
        
        //intitialising the request object to be sent to the API 
        QuestioningRequests.CompletedQuestionRoundRequest request = new QuestioningRequests.CompletedQuestionRoundRequest()
        {
            Difficulty = Difficulty,
            UserId = StudentId,
            Topic = Topic,
            TimeCompleted = time,
            questions =  AnsweredQuestions
        };
        
        //sending the request to the API to store the round of questioning in the database
        Console.WriteLine("Trying to send therequest");
       
        HttpResponseMessage response = await Http.PostAsJsonAsync("http://localhost:5148/api/Questions/AddShortTermData", request);
 
        NavigationManager.NavigateTo("/RoundComplete");
    }
    
    private void ResetArray()//this resets the array responsibel for tracking answers in the UI
    {
        for (int i = 0; i < 10; i++)
        {
            CorrectAnswers[i] = 0;
        }
        questionNum = 0;
    }

    private async Task<string> GetId()
    {
        ProtectedBrowserStorageResult<string> Id = await SessionStorage.GetAsync<string>("Id");
        if (Id.Success)
        {
            Console.WriteLine("There is an ID");
            return Id.Value;
        }
        return null;
    }
}

