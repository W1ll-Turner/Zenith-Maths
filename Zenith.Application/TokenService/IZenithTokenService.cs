using Zenith.Models.Account;

namespace Zenith.Application.TokenService;

public interface IZenithTokenService
{
    string CreateToken(string Id);
}