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

    protected override async Task OnInitializedAsync()
    {
        Task<string> UserId =  GetId();
        
        string Address = "http://localhost:5148/api/Questions/GetMostRecentQuestionRound/" + Convert.ToString(UserId);
        Console.WriteLine(Address);
        HttpResponseMessage response = await Http.GetAsync(Address); 
        
        
    }


    private async Task<string> GetId()
    {
        string ID;
        try
        {
            ProtectedBrowserStorageResult<string> Id = await SessionStorage.GetAsync<string>("Id");
            ID = Id.Value;
        }
        catch (Exception e)
        {
            ID = "0";
        }
        
        return ID;
    }
    
    
    
    
    
    
    
}