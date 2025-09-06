using Microsoft.AspNetCore.Http;

namespace Core.Domain.Enums;

public enum StatusCodesEnum
{
    Success = StatusCodes.Status200OK,
    Created = StatusCodes.Status201Created,
    Accepted = StatusCodes.Status202Accepted,

    // Client Error Codes
    BadRequest = StatusCodes.Status400BadRequest,
    Unauthorized = StatusCodes.Status401Unauthorized,
    Forbidden = StatusCodes.Status403Forbidden,
    NotFound = StatusCodes.Status404NotFound,
    Conflict = StatusCodes.Status409Conflict,

    // Server Error Codes
    InternalServerError = StatusCodes.Status500InternalServerError,
    NotImplemented = StatusCodes.Status501NotImplemented,
    BadGateway = StatusCodes.Status502BadGateway
}