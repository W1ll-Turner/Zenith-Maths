namespace Zenith.Contracts.Request;

public class SignUpRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Username { get; init; }
    public required string Fullname { get; init; }
    public required string Classcode { get; init; }
}
public class LoginRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}