using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Zenith.Contracts.Request.Account;
using Zenith.Contracts.Response;
using Zenith.Models.Account;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Questions;
public partial class RoundCompletePage : ComponentBase
{
    public IEnumerable<QuestionModels.AnsweredQuestion> Questions {get; set;}
    
    bool readyToDisplay = false;
    private bool authenticated = false;
    protected override async Task OnInitializedAsync()
    {
        
        
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender) //This is getting the user's ID from local storage, to make sure it is ready to be passed into the API calls
    {
        string Id = null;
        if (firstRender)
        {
            Id = await GetId();
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
        try
        {
            string Address = "http://localhost:5148/api/Questions/GetMostRecentQuestionRound/" + Id;
            Console.WriteLine(Address);
            HttpResponseMessage response = await Http.GetAsync(Address); 
            QuestionStatisticResponses.MostRecentQuestionRoundResponse? body = await response.Content.ReadFromJsonAsync<QuestionStatisticResponses.MostRecentQuestionRoundResponse>();
            Questions = body.Questions;
            readyToDisplay = true;
            StateHasChanged();
        }catch(Exception e)
        {
            readyToDisplay = false;
        }

    }
    
    private async Task<string> GetId()
    {
        ProtectedBrowserStorageResult<string> Id = await SessionStorage.GetAsync<string>("Id");
        if (Id.Success)
        {
            return Id.Value;
        }
        else
        {
            return null;
        }
        
    }
}