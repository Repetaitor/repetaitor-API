using Application.Models;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;

namespace Core.Application.Interfaces.Services;

public interface IUserAuthorizationService
{
    Task<string?> SendVerificationCode(string email, int userId);

    Task<SendVerificationCodeResponse?> SignUpUser(SignUpUserRequest request);
    
    Task<VerifyEmailResponse?> VerifyEmail(string guid, string email, string code);
    Task<UserSignInResponse?> MakeUserSignIn(string identification, string password);
}