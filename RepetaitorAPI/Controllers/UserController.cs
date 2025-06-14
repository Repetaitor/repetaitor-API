using System.Security.Claims;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RepetaitorAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IUserService userService,
    IUserAuthorizationService userAuthorizationService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserController> logger) : ControllerBase
{
    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(SendVerificationCodeResponse), 200)]
    public async Task<IResult> SignUp([FromBody] SignUpUserRequest request)
    {
        logger.LogInformation("SignUp request: {request}", JsonConvert.SerializeObject(request));
        var resp = await userAuthorizationService.SignUpUser(request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(VerifyEmailResponse), 200)]
    public async Task<IResult> VerifyAuthCode([FromBody] VerifyEmailRequest request)
    {
        logger.LogInformation("VerifyAuthCode request: {request}", JsonConvert.SerializeObject(request));
        var resp = await userAuthorizationService.VerifyEmail(request.Guid, request.Email, request.Code);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(UserModal), 200)]
    public async Task<IResult> SignIn([FromBody] UserSignInRequest request)
    {
        logger.LogInformation("SignIn request: {request}", JsonConvert.SerializeObject(request));
        var resp = await userAuthorizationService.MakeUserSignIn(request.Email, request.Password);
        if (resp.Code != StatusCodesEnum.Success || resp.Data == null)
            return Results.NotFound();
        await httpContextAccessor.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(resp.Data.ClaimsIdentity))!;
        return Results.Ok(resp.Data.User);
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
        }
        catch (Exception e)
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
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(UserModal), 200)]
    public async Task<IResult> GetUserInfoById(int userId)
    {
        logger.LogInformation("GetUserInfoById request: {userId}", userId);
        var resp = await userService.GetUserDefaultInfoAsync(userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(StudentDashboardHeaderInfoViewModel), 200)]
    public async Task<IResult> GetStudentDashboardInfo(int userId)
    {
        logger.LogInformation("GetStudentDashboardInfo request: {userId}", userId);
        var resp = await userService.GetStudentDashboardHeaderInfoAsync(userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(UserPerformanceViewModel), 200)]
    public async Task<IResult> GetStudentPerformanceInfoByDate(int userId, DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        logger.LogInformation("GetStudentPerformanceInfoByDate request: {userId}, {fromDate}, {toDate}", userId, fromDate, toDate);
        var resp = await userService.GetUserPerformanceAsync(userId, fromDate, toDate);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(TeacherDashboardBaseInfo), 200)]
    public async Task<IResult> GetTeacherDashboardInfo()
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await userService.GetTeacherDashboardHeaderInfoAsync(userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GroupsPerformance), 200)]
    public async Task<IResult> GetTeacherGroupsPerformanceByDate(DateTime? fromDate = null, DateTime? toDate = null)
    {
        logger.LogInformation("GetTeacherGroupsPerformanceByDate request: {fromDate}, {toDate}", fromDate, toDate);
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await userService.GetTeacherGroupsPerformanceByDate(userId, fromDate, toDate);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
}