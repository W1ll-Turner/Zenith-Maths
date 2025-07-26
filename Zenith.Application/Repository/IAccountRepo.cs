using Zenith.Models.Account;

namespace Zenith.Application.Repository;

public interface IAccountRepo //This interface define the data retrival methods related accounts
{
    // C make an accounty
    //R Log in 
    //U update account info (not nescary 
    //D delete account 

    Task<bool> CreateAccount(SignUp account);

    Task<int> LogIn(LogIn account);
    
    Task<account> GetAccountInfo(string accountId); 
    
    Task<bool> UpdateAccount(SignUp account);
    
    Task<bool> DeleteAccount(string accountId);
    




}