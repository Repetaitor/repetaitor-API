using System.ComponentModel.DataAnnotations;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.UserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EssayController(
    IEssayService essayService,
    IJWTTokenGenerator tokenGenerator,
    IHttpContextAccessor httpContextAccessor)
{
    [Authorize(Roles = "Teacher")]
    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<EssayModal>> AddNewEssay([FromBody] CreateNewEssayRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<EssayModal>()
            {
                Code = -1,
                Message = "You do not have permission to add this essay.",
            };
        return await essayService.CreateNewEssay(request.EssayTitle, request.EssayDescription,
            request.ExpectedWordCount, request.UserId);
    }
    [Authorize(Roles = "Teacher")]
    [Authorize(Roles = "Teacher")]
    [HttpDelete("[Action]")]
    public async Task<ResponseViewModel<ResultResponse>> DeleteEssay([FromQuery] int essayId, [FromQuery] int userId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<ResultResponse>()
            {
                Code = -1,
                Message = "You do not have permission to delete this essay.",
            };
        return await essayService.DeleteEssay(essayId, userId);
    }
    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    public async Task<ResponseViewModel<EssayModal>> UpdateEssay([FromBody] UpdateEssayRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<EssayModal>()
            {
                Code = -1,
                Message = "You do not have permission to update this essay.",
            };
        return await essayService.UpdateEssay(request.EssayId, request.EssayTitle, request.EssayDescription,
            request.ExpectedWordCount, request.UserId);
    }
    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<List<EssayModal>>> GetUserEssay([FromQuery] int userId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<List<EssayModal>>()
            {
                Code = -1,
                Message = "You do not have permission to delete this essay.",
            };
        return await essayService.GetUserEssays(userId);
    }
    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<EssayModal>> GetEssayById([FromQuery] int essayId)
    {
        return await essayService.GetEssayById(essayId);
    }
}