using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Zenith.Contracts.Request;
using Zenith.Contracts.Request.Account;

namespace ZenithFrontEnd.Components.Pages.Start;

public partial class SignUp : ComponentBase // inheritacne from the framwokr to let the code interface with HTML
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string ConfirmPassword { get; set; }
    
    public required string ClassCode { get; set; }

    private bool Error { get; set; } = false;
    private async void Submit()
    {
        bool Match = false;
        try
        {
            Match = StringMatch();
        }
        catch (Exception e) //Some invalid inputs have been given
        {
            Error = true;
        }
        
        if (Match == true && Password == ConfirmPassword)
        {
            SignUpRequest request = new SignUpRequest()
            {
                Username = Username,
                Password = Password,
                Email = Email,
                Classcode = ClassCode,
                Fullname = FirstName + " " + LastName,
            };
            
            //sending the sign up request
            Console.WriteLine("sending request");
            
            HttpResponseMessage response = await http.PostAsJsonAsync("http://localhost:5148/api/Account/SignUp", request);
            Console.WriteLine(response);
            
            if (response.IsSuccessStatusCode)
            {
                NavigationManager.NavigateTo("/Login");
            }
        }
        else
        {
            //will display the text detialing that the given credentails are incorrect
            Error = true; 
        }
        


    }

    private bool StringMatch() // this will be used to match all the feilds to the respecitve regex
    {
        const string usernamePattern = "[a-zA-Z_0-9]+"; //regex, for username, any value of a-z 0-9 with no spaces
        const string passwordPattern = "[a-zA-Z0-9]+"; //regex, for password. any numbre or letter with no spaces of underscores
        const string namePattern = "[A-Z][a-z]+"; //regex for a name, a capital lettyer followed by any number of a-z
        const string emailPattern = "^[A-Za-z0-9][A-Za-z1-9!#$%^&*_+=?`{}|~.]+[A-Za-z0-9!#$%^&*_+=?`{}|~.\\\\-]+[A-Za-z0-9]@[A-Za-z1-9][A-Za-z0-9\\\\-]+[A-Za-z0-9].[A-Za-z0-9.]+[A-Za-z0-9]$"; //regex for an email
        const string ClassCodePattern = "^[A-Za-z0-9]+$";
        
        //initialisi g the patterns in regex
        Regex ClassCodeRg = new Regex(ClassCodePattern);
        Regex userRg = new Regex(usernamePattern);
        Regex passwordRg = new Regex(passwordPattern);
        Regex nameRg = new Regex(namePattern);
        Regex emailRg = new Regex(emailPattern);

        if (userRg.IsMatch(Username) && passwordRg.IsMatch(Password) && nameRg.IsMatch(FirstName) &&
            emailRg.IsMatch(Email) && nameRg.IsMatch(LastName) && ClassCodeRg.IsMatch(ClassCode) ) //if all the string match
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}