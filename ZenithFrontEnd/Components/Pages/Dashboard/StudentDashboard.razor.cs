using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;

namespace ZenithFrontEnd.Components.Pages.Dashboard;

public partial class StudentDashboard : ComponentBase
{
    
    private LineChart lineChart = default!;
    private LineChartOptions lineChartOptions = default!;
    private ChartData chartData = default!;
    public bool FinishedRendering = false;

    protected async override Task OnInitializedAsync()
    {
        string Address = "http://localhost:5148/api/Questions/GetAllweeklySummarys/" + "20";
        HttpResponseMessage response = await Http.GetAsync(Address);
        IEnumerable<WeeklySummary> Summaries = await response.Content.ReadFromJsonAsync<IEnumerable<WeeklySummary>>();
        
        
        
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

        lineChartOptions.Plugins.Title!.Text = "Completion";
        lineChartOptions.Plugins.Title.Display = true;
        FinishedRendering = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        
        
        if (!firstRender)
        {
            return;
        }

        await Task.Delay(500);

        await lineChart.InitializeAsync(chartData, lineChartOptions);
        StateHasChanged();
    }

    
    
    
    
    
}