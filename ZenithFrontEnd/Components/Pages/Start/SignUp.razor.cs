using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace ZenithFrontEnd.Components.Pages.Start;

public partial class SignUp : ComponentBase // inheritacne from the framwokr to let the code interface with HTML
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string ConfirmPassword { get; set; }

    public bool Error { get; set; } = false;
    private void Submit()
    {
        bool Match = StringMatch();
        if (Match == true && Password == ConfirmPassword)
        {
            //account will be made and sent to the API
            //move to log in screen 
        }
        else
        {
            Error = true; //will display the text detialing that the given credentails are incorrect
        }


    }

    private bool StringMatch() // this will be used to match all the feilds to the respecitve regex
    {
        const string usernamePattern = "[a-zA-Z_0-9]+"; //regex, for username, any value of a-z 0-9 with no spaces
        const string passwordPattern = "[a-zA-Z0-9]+"; //regex, for password. any numbre or letter with no spaces of underscores
        const string namePattern = "[A-Z][a-z]+"; //regex for a name, a capital lettyer followed by any number of a-z
        const string emailPattern = "^[A-Za-z0-9][A-Za-z1-9!#$%^&*_+=?`{}|~.]+[A-Za-z0-9!#$%^&*_+=?`{}|~.\\\\-]+[A-Za-z0-9]@[A-Za-z1-9][A-Za-z0-9\\\\-]+[A-Za-z0-9].[A-Za-z0-9.]+[A-Za-z0-9]$"; //regex for an email

        Regex userRg = new Regex(usernamePattern);
        Regex passwordRg = new Regex(passwordPattern);
        Regex nameRg = new Regex(namePattern);
        Regex emailRg = new Regex(emailPattern);

        if (userRg.IsMatch(Username) && passwordRg.IsMatch(Password) && nameRg.IsMatch(FirstName) &&
            emailRg.IsMatch(Email) && nameRg.IsMatch(LastName)) //if all the string match
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}