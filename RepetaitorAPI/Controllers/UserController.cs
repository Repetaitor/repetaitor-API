using System.ComponentModel.DataAnnotations;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Core.Application.Models.DTO.UserInfo;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService, IUserAuthorizationService userAuthorizationService)
{
    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(SendVerificationCodeResponse), 200)]
    public async Task<IResult> SignUp([FromBody] SignUpUserRequest request)
    {
        var resp = await userAuthorizationService.SignUpUser(request);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(VerifyEmailResponse), 200)]
    public async Task<IResult> VerifyAuthCode([FromBody] VerifyEmailRequest request)
    {
        var resp= await userAuthorizationService.VerifyEmail(request.Guid, request.Email, request.Code);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(UserSignInResponse), 200)]
    public async Task<IResult> SignIn([FromBody] UserSignInRequest request)
    {
        var resp = await userAuthorizationService.MakeUserSignIn(request.Email, request.Password);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(UserModal), 200)]
    public async Task<IResult> GetUserBaseInfo([FromQuery] int userId)
    {
        var resp = await userService.GetUserDefaultInfoAsync(userId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }
}