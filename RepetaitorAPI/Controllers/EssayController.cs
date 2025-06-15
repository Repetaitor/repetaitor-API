using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.UserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RepetaitorAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EssayController(
    IEssayService essayService,
    IJWTTokenGenerator tokenGenerator,
    IHttpContextAccessor httpContextAccessor,
    ILogger<EssayController> logger) : ControllerBase
{
    [Authorize(Roles = "Teacher")]
    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(EssayModal), 200)]
    public async Task<IResult> AddNewEssay([FromBody] CreateNewEssayRequest request)
    {
        logger.LogInformation("SignUp request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await essayService.CreateNewEssay(request.EssayTitle, request.EssayDescription,
            request.ExpectedWordCount, userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("[Action]")]
    public async Task<IResult> DeleteEssay([FromQuery] int essayId)
    {
        logger.LogInformation("SignUp request: {essayId}", essayId);
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await essayService.DeleteEssay(essayId, userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    [ProducesResponseType(typeof(EssayModal), 200)]
    public async Task<IResult> UpdateEssay([FromBody] UpdateEssayRequest request)
    {
        logger.LogInformation("SignUp request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await essayService.UpdateEssay(request.EssayId, request.EssayTitle, request.EssayDescription,
            request.ExpectedWordCount, userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<EssayModal>), 200)]
    public async Task<IResult> GetUserEssay()
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await essayService.GetUserEssays(userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(EssayModal), 200)]
    public async Task<IResult> GetEssayById([FromQuery] int essayId)
    {
        logger.LogInformation("SignUp request: {essayId}", essayId);
        var resp = await essayService.GetEssayById(essayId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
}