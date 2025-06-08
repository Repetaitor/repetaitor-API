using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ProjectServices.Implementations;

public class UserAuthorizationService(
    IUserRepository userRepository,
    IAuthCodesRepository authCodesRepository,
    IJWTTokenGenerator jwtTokenGenerator,
    IMailService mailService,
    IImagesStoreService imagesStoreService) : IUserAuthorizationService
{
    private static string GetHashedPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var builder = new StringBuilder();
        foreach (var sByte in bytes)
        {
            builder.Append(sByte.ToString("x2"));
        }

        return builder.ToString();
    }

    public async Task<ResponseView<string>> SendVerificationCode(string email, int userId)
    {
        try
        {
            var generatedCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var result = await mailService.SendAuthMail(email, "Authentication Code", generatedCode);
            if (!result)
                return new ResponseView<string>()
                {
                    Code = StatusCodesEnum.InternalServerError,
                    Message = "Failed to send verification code.",
                    Data = null
                };
            var guid = await authCodesRepository.CreateAuthCode(generatedCode, email, userId);
            return new ResponseView<string>()
            {
                Code = StatusCodesEnum.Success,
                Message = "Verification code sent successfully.",
                Data = guid
            };
        } catch (Exception ex)
        {
            return new ResponseView<string>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<VerifyEmailResponse>> VerifyEmail(string guid, string email, string code)
    {
        try
        {
            var res = await authCodesRepository.CheckAuthCode(guid, email, code);
            return new ResponseView<VerifyEmailResponse>
            {
                Code = StatusCodesEnum.Success,
                Data = new VerifyEmailResponse
                {
                    Verified = res
                }
            };
        } catch (Exception ex)
        {
            return new ResponseView<VerifyEmailResponse>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<SendVerificationCodeResponse>> SignUpUser(SignUpUserRequest request)
    {
        try
        {
            var isEmailExist = await userRepository.EmailExists(request.Email);
            if (isEmailExist)
                return new ResponseView<SendVerificationCodeResponse>()
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "Email already exists.",
                    Data = null
                };
            if (request.Role != "Student" && request.Role != "Teacher")
                return new ResponseView<SendVerificationCodeResponse>()
                {
                    Code = StatusCodesEnum.BadRequest,
                    Message = "Invalid role specified.",
                    Data = null
                };
            var result = await userRepository.AddUser(request.FirstName, request.LastName, request.Email,
                GetHashedPassword(request.Password), request.Role);
            if (result == -1)
                return new ResponseView<SendVerificationCodeResponse>()
                {
                    Code = StatusCodesEnum.InternalServerError,
                    Message = "Failed to create user.",
                    Data = null
                };
            var guid = await SendVerificationCode(request.Email, result);
            if (guid is { Code: 0, Data: not null })
            {
                return new ResponseView<SendVerificationCodeResponse>()
                {
                    Code = StatusCodesEnum.Success,
                    Message = "Verification code sent successfully.",
                    Data = new SendVerificationCodeResponse
                    {
                        Guid = guid.Data
                    }
                };
            }
            imagesStoreService.CreateDirectoryForUser(result);
            return new ResponseView<SendVerificationCodeResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "Failed to send verification code.",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<SendVerificationCodeResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<UserSignInResponse>> MakeUserSignIn(string identification, string password)
    {
        try
        {
            var result = await userRepository.CheckIfUser(identification, GetHashedPassword(password));
            var claims = new List<Claim>
            {
                new("email", result.Email),
                new(ClaimTypes.NameIdentifier, result.Id.ToString()),
                new(ClaimTypes.Role, result.Role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            return new ResponseView<UserSignInResponse>()
            {
                Code = StatusCodesEnum.Success,
                Message = "User signed in successfully.",
                Data = new UserSignInResponse()
                {

                    User = result,
                    ClaimsIdentity = claimsIdentity
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<UserSignInResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}