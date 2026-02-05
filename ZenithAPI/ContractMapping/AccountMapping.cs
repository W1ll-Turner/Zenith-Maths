using Zenith.Contracts.Request;
using Zenith.Contracts.Request.Account;
using Zenith.Models.Account;
namespace ZenithAPI.ContractMapping;

public static class AccountMapping
{
    public static SignUp MapFromSignUpRequest(this SignUpRequest request)
    {
        return new SignUp
        {
            Email = request.Email,
            Password = request.Password,
            Username = request.Username,
            Fullname = request.Fullname,
            ClassCode = request.Classcode
        };
    }

    public static LogIn MapFromLogInRequest(this LoginRequest request)
    {
        return new LogIn
        {
            Password = request.Password,
            Username = request.Username,
        };
    }
}