using Microsoft.AspNetCore.Components;

namespace ZenithFrontEnd.Components.Pages.Questions;

public partial class QuestionPage : ComponentBase
{
    public string Id { get; set; }
    public bool authenticated {get; set;}


    protected override async Task OnAfterRenderAsync(bool firstRender) //This is getting the user's ID from local storage, to make sure it is ready to be passed into the API calls
    {
        if (firstRender)
        {
            try
            {
                Id = await LocalStorage.GetItemAsync<string>("Id");
            }
            catch (Exception ex)
            {
                
            }
            
            StateHasChanged();
        }
    }
}