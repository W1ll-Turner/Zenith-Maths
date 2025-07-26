namespace Zenith.Contracts.Request.Account;

public class SignUpRequest
{
    public required string email { get; init; }
    public required string password { get; init; }
    public required string username { get; init; }
    public required string fullname { get; init; }
    public required string classcode { get; init; }
    
}

public class LoginRequest
{
    public required string username { get; init; }
    public required string password { get; init; }
}