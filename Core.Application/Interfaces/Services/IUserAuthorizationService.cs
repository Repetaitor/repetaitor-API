using Application.Models;
using Core.Application.Models;
using Core.Application.Models.DTO.Authorization;

namespace Core.Application.Interfaces.Services;

public interface IUserAuthorizationService
{
    Task<ResponseViewModel<SendVerificationCodeResponse>> SendVerificationCode(string email);

    Task<ResponseViewModel<SignUpUserResponse>> SignUpUser(SignUpUserRequest request);

    Task<ResponseViewModel<VerifyEmailResponse>> VerifyEmail(string guid, string email, string code);
    Task<ResponseViewModel<UserSignInResponse>> MakeUserSignIn(string identification, string password);
}