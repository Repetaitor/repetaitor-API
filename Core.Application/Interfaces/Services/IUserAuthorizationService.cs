using Application.Models;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;

namespace Core.Application.Interfaces.Services;

public interface IUserAuthorizationService
{
    Task<ResponseView<string>> SendVerificationCode(string email, int userId);

    Task<ResponseView<SendVerificationCodeResponse>> SignUpUser(SignUpUserRequest request);
    
    Task<ResponseView<VerifyEmailResponse>> VerifyEmail(string guid, string email, string code);
    Task<ResponseView<UserSignInResponse>> MakeUserSignIn(string identification, string password);
}