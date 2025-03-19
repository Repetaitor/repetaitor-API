using System.ComponentModel.DataAnnotations;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Core.Application.Models.DTO.UserInfo;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService, IUserAuthorizationService userAuthorizationService)
{
    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<SendVerificationCodeResponse>> SignUp([FromBody] SignUpUserRequest request)
    {
        return await userAuthorizationService.SignUpUser(request);
    }

    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<VerifyEmailResponse>> VerifyAuthCode([FromBody] VerifyEmailRequest request)
    {
        return await userAuthorizationService.VerifyEmail(request.Guid, request.Email, request.Code);
    }

    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<UserSignInResponse>> SignIn([FromBody] UserSignInRequest request)
    {
        return await userAuthorizationService.MakeUserSignIn(request.Email, request.Password);
    }

    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<UserModal>> GetUserBaseInfo([FromQuery] int userId)
    {
        return await userService.GetUserDefaultInfoAsync(userId);
    }
}