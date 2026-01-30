using Microsoft.AspNetCore.Components;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Questions;

public partial class QuestionSelectionPage : ComponentBase
{
    public string Id { get; set; }
    private bool authenticated  = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender) //This is getting the user's ID from local storage, to make sure it is ready to be passed into the API calls
    {
        if (firstRender)
        {
            //getting the ID of the user fromffffnc<string>("Id")).Value ??"";
            authenticated = Id != "" ;
            Console.WriteLine(Id);
            
            
            
            StateHasChanged();
        }
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