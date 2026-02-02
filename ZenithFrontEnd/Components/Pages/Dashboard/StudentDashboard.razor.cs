using System.Net.WebSockets;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Zenith.Models.Account;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Dashboard;

public partial class StudentDashboard : ComponentBase
{
    
    //these are the properties required for the graph to be duisplayed 
    private LineChart lineChart = default!;
    private LineChartOptions lineChartOptions = default!;
    private ChartData chartData = default!;
    
    //will make sure the page does not try to redner without the required information first, prevenmting a null exception being thrown 
    public bool FinishedRendering = false;
    
    //stores the most recent stat so they cna be displayed in more detial 
    WeeklySummary MostRecentStats {get; set;}
    
    //stores every collection of long terms stats
    private IEnumerable<WeeklySummary>? Summaries { get; set; }
    //stores all of the recently answered rounds of questioning 
    private List<CompletedRoundOfQuestioning>? QuestioningRounds { get; set; }
    //stores the round of questioning the user would like to see in more detial 
    private IEnumerable<QuestionModels.AnsweredQuestion> CurrentQuestioningRound {get; set;}
    //tracks whether to open the window that will track the most recent round of quesitoning
    private bool displayQuestionRound = false;
    private bool authenticated = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        
        if (!firstRender)
        {
            return;
        }

        string? Id = await GetId();
        if (Id == null)
        {
            authenticated = false;
            return;
        }
        authenticated = true;
        
        string weeklySummariesAddress = "http://localhost:5148/api/Questions/GetAllweeklySummarys/" + "70";
        HttpResponseMessage summariesResponse = await Http.GetAsync(weeklySummariesAddress);
        Summaries = await summariesResponse.Content.ReadFromJsonAsync<IEnumerable<WeeklySummary>>();
        
        string questioningRoundsAddress = "http://localhost:5148/api/Questions/GetAllQuestioningRounds/" + "820";
        HttpResponseMessage questionRoundsResponse = await Http.GetAsync(questioningRoundsAddress);
        QuestioningRounds = await questionRoundsResponse.Content.ReadFromJsonAsync<List<CompletedRoundOfQuestioning>>();

        MostRecentStats = Summaries.Last();

        foreach (CompletedRoundOfQuestioning round in QuestioningRounds)
        {
            Console.WriteLine("Question????");
            IEnumerable<QuestionModels.AnsweredQuestion> answeredQuestions = round.answeredQuestions;
            foreach (QuestionModels.AnsweredQuestion answeredQuestion in answeredQuestions)
            {
                Console.WriteLine("Data please");
                Console.WriteLine(answeredQuestion.Question);
            }
        }
        
        InitialiseGraph();
        
        
        await Task.Delay(500);

        await lineChart.InitializeAsync(chartData, lineChartOptions);
        StateHasChanged();
    }

    private void InitialiseGraph()
    {
        var colors = ColorUtility.CategoricalTwelveColors;

        List<string> labels = new List<string>();
        List<double?> data = new List<double?>();
        foreach (WeeklySummary summary in Summaries)
        {
            
            labels.Add(summary.weekNumber);
            data.Add(summary.completion);

        }
        
        var datasets = new List<IChartDataset>();
        
        
        
        var dataset1 = new LineChartDataset
        {
            Label = "Completion",
            Data = data, 
            BackgroundColor = colors[0],
            BorderColor = colors[0],
            BorderWidth = 2,
            HoverBorderWidth = 4,
            // PointBackgroundColor = colors[0],
            // PointRadius = 0, // hide points
            // PointHoverRadius = 4,
        };
        datasets.Add(dataset1);
        chartData = new ChartData { Labels = labels, Datasets = datasets };

        lineChartOptions = new();
        lineChartOptions.Responsive = true;
        lineChartOptions.Interaction = new Interaction { Mode = InteractionMode.Index };

        lineChartOptions.Scales.X!.Title = new ChartAxesTitle { Text = "Weeks", Display = true };
        lineChartOptions.Scales.Y!.Title = new ChartAxesTitle { Text = "Completion", Display = true };
        lineChartOptions.Scales.Y.Max = 1.0;
        lineChartOptions.Scales.Y.Min = 0.0;
        
        FinishedRendering = true;
        
        
    }

    private void DisplayRoundOfQuestioning(int roundIndex)
    {
        Console.WriteLine(roundIndex);
        CurrentQuestioningRound = QuestioningRounds[roundIndex].answeredQuestions;
        displayQuestionRound = true;
        
        StateHasChanged();
    }

    private void NavigateToSelection()
    {
        NavigationManager.NavigateTo($"/QuestionSelection");
    }

    private void Close()
    {
        displayQuestionRound = false;
        StateHasChanged();
    }
    
    private async Task<string?> GetId()
    {
        ProtectedBrowserStorageResult<string?> Id = await SessionStorage.GetAsync<string>("Id");
        if (Id.Success)
        {
            return Id.Value;
        }
        
        return null;
    }
    
}