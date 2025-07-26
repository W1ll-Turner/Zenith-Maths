using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Zenith.Application.Repository;
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

    public AccountController(IAccountRepo accountRepo) //loosely coupling the account repository for the data access capabilites 
    {
        _accountRepo = accountRepo;
    }


    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {

        SignUp account = request.MapFromSignUpRequest();
        Console.WriteLine("mapped");
        bool success = await _accountRepo.CreateAccount(account);
        Console.WriteLine("added to db");
        return success ? Ok() : BadRequest();

    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LogIn account = request.MapFromLogInRequest();
        int id = await _accountRepo.LogIn(account);
        if (id == 0)
        {
            return Unauthorized();
        }
        else
        {
            //create token
        }

    throw new NotImplementedException();

    }
    
    
    
}