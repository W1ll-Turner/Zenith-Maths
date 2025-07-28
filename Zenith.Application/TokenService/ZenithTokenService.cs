using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Zenith.Models.Account;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Zenith.Application.TokenService;

public class ZenithTokenService : IZenithTokenService
{
    private readonly IConfiguration _config;

    public ZenithTokenService(IConfiguration config) //injecting the appsittings in the token generator 
    {
        _config = config;
    }
    
    public string CreateToken(string Id)
    {
        var claims = new List<Claim> //adding the userId to the JWT to uniquely the user holding the token
        {
            new Claim(ClaimTypes.NameIdentifier, Id)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now
                .AddHours(12), //making the token valid for 12 hours (the user will be logged in for 12 hours 
            signingCredentials: creds

        );
        
        return new JwtSecurityTokenHandler().WriteToken(token); //converting the token "bluepirnt" into na actualy jwt web token


    }
}