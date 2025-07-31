


using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

using Zenith.Contracts.Request.Account;

namespace ZenithFrontEnd.Components.Pages.Start;

public partial class Login : ComponentBase //inheritance from ASP.NET Framework allowing for the code to be linked into the HTML file
{
    
    public required string Username { get; set; } = ""; 
    public required string Password { get; set; } = "";
    public bool Error { get; set; } = false;

    private async Task Submit()
    {
        string usernamePattern = "[a-zA-Z_0-9]+";   //Regex to match a Username, (Any combination of a-z 0-9 with an underscore, no spaces)
        string passwordPattern = "[a-zA-Z0-9]+";// regex to match a Password. This is any combination of a-z 0-9 however with no spaces or underscores
        
        Regex userRg = new Regex(usernamePattern);
        Regex passwordRg = new Regex(passwordPattern);
        
        
        if (userRg.IsMatch(Username) && passwordRg.IsMatch(Password))
        {
            Console.WriteLine("match");
            //send request to API
            //move onto dashboard if true
            loginRequest request = new loginRequest()
            {
                username = Username,
                password = Password
            };
            Console.WriteLine(request.password);
            Console.WriteLine(request.username);
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5148/api/");
            HttpResponseMessage response = await client.PostAsJsonAsync("Account/Login", request);
            if (response.IsSuccessStatusCode)
            {
               string Id = response.Content.ReadAsStringAsync().Result;
               Console.WriteLine(Id);
        
            }
            else
            {
                Console.WriteLine("no work");
            }
            
            
            
           


        }
        else //This will display the message that they have given an invalid username and password
        {
            Error = true;
            return;
        }
        
        
        
        
        
        
        
        
        
        
    }

    
    
}