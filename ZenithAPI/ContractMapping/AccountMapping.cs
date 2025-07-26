using Zenith.Contracts.Request.Account;
using Zenith.Models.Account;

namespace ZenithAPI.ContractMapping;

public static class AccountMapping
{

    public static SignUp MapFromSignUpRequest(this SignUpRequest request)
    {
        return new SignUp
        {
            Email = request.email,
            Password = request.password,
            Username = request.username,
            Fullname = request.fullname,
            ClassCode = request.classcode
        };
        

    }

    public static LogIn MapFromLogInRequest(this LoginRequest request)
    {
        return new LogIn
        {
            Password = request.password,
            Username = request.username,
        };
    }
    
    
}