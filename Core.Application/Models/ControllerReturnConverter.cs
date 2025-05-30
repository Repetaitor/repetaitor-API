using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Models;

public static class ControllerReturnConverter
{
    public static IResult ConvertToReturnType<T>(ResponseView<T> response)
    {
        return response.Code switch
        {
            StatusCodesEnum.Success => Results.Ok(response.Data),
            StatusCodesEnum.Created => Results.Created(string.Empty, response.Data),
            StatusCodesEnum.Accepted => Results.Accepted(string.Empty, response.Data),

            StatusCodesEnum.BadRequest => Results.BadRequest(response.Message),
            StatusCodesEnum.Unauthorized => Results.Unauthorized(),
            StatusCodesEnum.Forbidden => Results.StatusCode((int)StatusCodesEnum.Forbidden),
            StatusCodesEnum.NotFound => Results.NotFound(response.Message),
            StatusCodesEnum.Conflict => Results.Conflict(response.Message),

            StatusCodesEnum.InternalServerError => Results.StatusCode((int)StatusCodesEnum.InternalServerError),
            StatusCodesEnum.NotImplemented => Results.StatusCode((int)StatusCodesEnum.NotImplemented),
            StatusCodesEnum.BadGateway => Results.StatusCode((int)StatusCodesEnum.BadGateway),

            _ => Results.StatusCode((int)StatusCodesEnum.InternalServerError)
        };
    }
}