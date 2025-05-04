using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Core.Application.Models.DTO.UserInfo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService, IUserAuthorizationService userAuthorizationService, IHttpContextAccessor httpContextAccessor)
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
    [ProducesResponseType(typeof(UserModal), 200)]
    public async Task<IResult> SignIn([FromBody] UserSignInRequest request)
    {
        var resp = await userAuthorizationService.MakeUserSignIn(request.Email, request.Password);
        if(resp == null)
            return Results.Problem();
        await httpContextAccessor.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(resp.ClaimsIdentity))!;
        return Results.Ok(resp.User);
    }
    [Authorize]
    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(UserModal), 200)]
    public async Task<IResult> SignOut()
    {
        try
        {
            await httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)!;
            return Results.Ok();
        } catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(UserModal), 200)]
    public async Task<IResult> Me()
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await userService.GetUserDefaultInfoAsync(userId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }
}