using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Questions;

public partial class QuestionSelectionPage : ComponentBase
{

    private bool authenticated  = false;
    
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


    
    private async Task<string> GetId()
    {
        ProtectedBrowserStorageResult<string> Id = await SessionStorage.GetAsync<string>("Id");
        if (Id.Success)
        {
            return Id.Value;
        }
        
        return null;
    }



    private void RouteAddition()
    {
        NavigationManager.NavigateTo($"/Questions/addition");
        
    }

    private void RouteSubtraction()
    {
        NavigationManager.NavigateTo($"/Questions/subtraction");
        
    }

    private void RouteMultiplication()
    {
        NavigationManager.NavigateTo($"/Questions/multiplication");
    }

    private void RouteDivision()
    {
        NavigationManager.NavigateTo($"/Questions/division");
        
    }
    
    private void RouteQuadratic()
    {
        NavigationManager.NavigateTo($"/Questions/quadratic");
    }

    private void RouteCollectingTerms()
    {
        NavigationManager.NavigateTo($"/Questions/collectingterms");
    }

    
    
    
    
}