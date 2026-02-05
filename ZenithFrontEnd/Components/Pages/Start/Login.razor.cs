using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Zenith.Contracts.Request;
using Zenith.Contracts.Request.Account;

namespace ZenithFrontEnd.Components.Pages.Start;

public partial class Login : ComponentBase //inheritance from ASP.NET Framework allowing for the code to be linked into the HTML file
{
    public required string Username { get; set; } = ""; 
    public required string Password { get; set; } = "";
    public bool Error { get; set; } = false;
    public bool authenticated = false;
    private async Task Submit()
    {
        string usernamePattern = "[a-zA-Z_0-9]+";   //Regex to match a Username, (Any combination of a-z 0-9 with an underscore, no spaces)
        string passwordPattern = "[a-zA-Z0-9]+";// regex to match a Password. This is any combination of a-z 0-9 however with no spaces or underscores
        
        // initialising the regex patterns
        Regex userRg = new Regex(usernamePattern);
        Regex passwordRg = new Regex(passwordPattern);
        
        //mathcing the patterns so data is in a valid form 
        if (userRg.IsMatch(Username) && passwordRg.IsMatch(Password))
        {
            LoginRequest request = new LoginRequest() //making the log in request object
            {
                Username = Username,
                Password = Password
            };
            
            //making the log in request to the API 
            Http.BaseAddress = new Uri("http://localhost:5148/api/");
            HttpResponseMessage response = await Http.PostAsJsonAsync("Account/Login", request); 
            if (response.IsSuccessStatusCode)//if log in was succsefful
            {
               string id = response.Content.ReadAsStringAsync().Result; //getting the USer's ID from the response
               await SessionStorage.SetAsync("Id", id);
               
               NavigationManager.NavigateTo("/QuestionSelection");
            }
            else
            {
                Error = true; // username or password were incorrect
            }
        }
        else //This will display the message that they have given an invalid username and password
        {
            Error = true;
        }
    }
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
        else
        {
            return null;
        }
        
    }
    
    
}