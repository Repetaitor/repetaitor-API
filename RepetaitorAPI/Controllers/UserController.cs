using System.ComponentModel.DataAnnotations;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService, IUserAuthorizationService userAuthorizationService)
{
    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<SendVerificationCodeResponse>> SendVerificationCode([FromBody] SendVerificationCodeRequest request)
    {
        return await userAuthorizationService.SendVerificationCode(request.Email);
    }
    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<SignUpUserResponse>> SignUpUser([FromBody] SignUpUserRequest request)
    {
        return await userAuthorizationService.SignUpUser(request);
    }

    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<VerifyEmailResponse>> CheckVerificationCode([FromBody] VerifyEmailRequest request)
    {
        return await userAuthorizationService.VerifyEmail(request.Guid, request.Email, request.Code);
    }

    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<UserSignInResponse>> UserSignIn([FromBody] UserSignInRequest request)
    {
        return await userAuthorizationService.MakeUserSignIn(request.Identifier, request.Password);
    }
}