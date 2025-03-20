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
    IMailService mailService) : IUserAuthorizationService
{
    private static string GetHashedPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var builder = new StringBuilder();
        foreach (var sByte in bytes)
        {
            builder.Append(sByte.ToString("x2"));
        }

        return builder.ToString();
    }

    public async Task<string?> SendVerificationCode(string email, int userId)
    {
        var generatedCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var result = await mailService.SendAuthMail(email, "Authentication Code", generatedCode);
        if (!result)
            return null;
        var guid = await authCodesRepository.CreateAuthCode(generatedCode, email, userId);
        return guid;
    }

    public async Task<ResponseViewModel<VerifyEmailResponse>> VerifyEmail(string guid, string email, string code)
    {
        var verified = await authCodesRepository.CheckAuthCode(guid, email, code);
        return new ResponseViewModel<VerifyEmailResponse>()
        {
            Code = verified ? 0 : -1,
            Message = verified ? "" : "Something went wrong",
            Data = new VerifyEmailResponse()
            {
                Verified = verified
            }
        };
    }

    public async Task<ResponseViewModel<SendVerificationCodeResponse>> SignUpUser(SignUpUserRequest request)
    {
        if (await userRepository.EmailExists(request.Email))
            return new ResponseViewModel<SendVerificationCodeResponse>()
            {
                Code = -1,
                Message = "Email Already Exists"
            };
        if (request.Role != "Student" && request.Role != "Teacher")
            return new ResponseViewModel<SendVerificationCodeResponse>()
            {
                Code = -1,
                Message = "Valid Role Required"
            };
        var result = await userRepository.AddUser(request.FirstName, request.LastName, request.Email,
            GetHashedPassword(request.Password), request.Role);
        if (result == -1)
            return new ResponseViewModel<SendVerificationCodeResponse>()
            {
                Code = -1,
                Message = "Something went wrong when adding user",
            };
        var guid = await SendVerificationCode(request.Email, result);
        if (guid != null)
        {
            return new ResponseViewModel<SendVerificationCodeResponse>()
            {
                Code = 0,
                Message = "",
                Data = new SendVerificationCodeResponse()
                {
                    Guid = guid,
                }
            };
        }

        return new ResponseViewModel<SendVerificationCodeResponse>()
        {
            Code = -1,
            Message = "Something went wrong when adding user",
        };
    }

    public async Task<ResponseViewModel<UserSignInResponse>> MakeUserSignIn(string identification, string password)
    {
        var result = await userRepository.CheckIfUser(identification, GetHashedPassword(password));
        if (result != null)
        {
            return new ResponseViewModel<UserSignInResponse>()
            {
                Code = 0,
                Message = "",
                Data = new UserSignInResponse()
                {
                    User = result,
                    JWTToken = jwtTokenGenerator.GenerateJWTtoken(new Claim[]
                    {
                        new Claim("email", result.Email), new Claim("userId", result.Id.ToString()),
                        new Claim(ClaimTypes.Role, result.Role),
                    }),
                }
            };
        }

        return new ResponseViewModel<UserSignInResponse>()
        {
            Code = -1,
            Message = "User Not Found"
        };
    }
}