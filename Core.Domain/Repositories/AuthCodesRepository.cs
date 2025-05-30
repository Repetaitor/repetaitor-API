using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;

namespace Core.Domain.Repositories;

public class AuthCodesRepository(ApplicationContext context, IUserRepository userRepository) : IAuthCodesRepository
{
    private ApplicationContext _context = context;

    public async Task<ResponseView<string>> CreateAuthCode(string code, string email, int userId)
    {
        try
        {
            var newGuid = Guid.NewGuid();
            var auth = new AuthenticationCodes()
            {
                UserId = userId,
                Code = code,
                Guid = newGuid.ToString(),
                Email = email,
                IsVerified = false
            };
            await _context.AuthCodes.AddAsync(auth);
            await _context.SaveChangesAsync();
            return new ResponseView<string>()
            {
                Code = StatusCodesEnum.Success,
                Data = newGuid.ToString(),
                Message = "Authentication code created successfully."
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<string>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<bool>> CheckAuthCode(string guid, string email, string code)
    {
        try
        {
            var cd = await _context.AuthCodes.FirstOrDefaultAsync(x =>
                x.Guid == guid && x.Email == email && x.IsVerified == false);
            if (cd == null || cd.Email != email || cd.Code != code)
                return new ResponseView<bool>()
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "Invalid authentication code.",
                    Data = false
                };
            if((DateTime.Now - cd.CreateDate).TotalMinutes > 10)
            {
                return new ResponseView<bool>()
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "Expired authentication code.",
                    Data = false
                };
            }

            cd.IsVerified = true;
            await _context.SaveChangesAsync();
            var res = await userRepository.ActivateUser(cd.UserId);
            return new ResponseView<bool>()
            {
                Code = res.Code,
                Message = res.Message,
                Data = res.Data
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<bool>> EmailIsVerified(string guid, string email)
    {
        try
        {
            var cd = await _context.AuthCodes.FirstOrDefaultAsync(x => x.Email == email && x.Guid == guid);
            if (cd == null)
            {
                return new ResponseView<bool>()
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Authentication code not found.",
                    Data = false
                };
            }
            if (cd.IsVerified)
            {
                return new ResponseView<bool>()
                {
                    Code = StatusCodesEnum.Success,
                    Message = "Email is verified.",
                    Data = true
                };
            }
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Conflict,
                Message = "Email is not verified.",
                Data = false
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }
}