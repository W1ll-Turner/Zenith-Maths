using Zenith.Models.Account;

namespace Zenith.Application.Repository;

public interface IAccountRepo //This interface define the data retrival methods related accounts
{
    Task<bool> CreateAccount(SignUp account);

    Task<int> LogIn(LogIn account);
}