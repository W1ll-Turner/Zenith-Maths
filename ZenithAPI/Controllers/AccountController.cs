using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Zenith.Application.Repository;
using Zenith.Application.TokenService;
using Zenith.Contracts.Request.Account;
using Zenith.Models.Account;
using ZenithAPI.ContractMapping;
using LoginRequest = Zenith.Contracts.Request.Account.LoginRequest;

namespace ZenithAPI.Controllers;
[ApiController]
[Route("api/Account")]
public class AccountController : ControllerBase //inhertiance from the ASP.NET Framework to enable the controller functionalilty 
{
    private readonly IAccountRepo _accountRepo;
    private readonly IZenithTokenService  _tokenService;
    public AccountController(IAccountRepo accountRepo, IZenithTokenService tokenService) //loosely coupling the account repository for the data access capabilites 
    {
        _tokenService = tokenService;
        _accountRepo = accountRepo;
    }


    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {

        SignUp account = request.MapFromSignUpRequest();
   
        bool success = await _accountRepo.CreateAccount(account); //adding the data to the database
        
        return success ? Ok() : BadRequest();

    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LogIn account = request.MapFromLogInRequest();
        int id = await _accountRepo.LogIn(account); //gets the id of the account
        if (id == 0)
        {
            return Unauthorized();
        }
        else
        {
            var s = id.ToString();
            var Token = _tokenService.CreateToken(s);
            return Ok(Token);
        }

    

    }
    
    
    
}