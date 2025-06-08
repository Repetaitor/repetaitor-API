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

    public async Task<string> CreateAuthCode(string code, string email, int userId)
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
        return newGuid.ToString();
    }

    public async Task<bool> CheckAuthCode(string guid, string email, string code)
    {
        var cd = await _context.AuthCodes.FirstOrDefaultAsync(x =>
            x.Guid == guid && x.Email == email && x.IsVerified == false);
        if (cd == null || cd.Email != email || cd.Code != code)
            throw new Exception("Invalid authentication code or email.");
        if ((DateTime.Now - cd.CreateDate).TotalMinutes > 10)
        {
            throw new Exception("Authentication code has expired.");
        }

        cd.IsVerified = true;
        await _context.SaveChangesAsync();
        var res = await userRepository.ActivateUser(cd.UserId);
        return res;
    }

    public async Task<bool> EmailIsVerified(string guid, string email)
    {
        var cd = await _context.AuthCodes.FirstOrDefaultAsync(x => x.Email == email && x.Guid == guid);
        if (cd == null)
        {
            throw new Exception("Authentication code not found for the provided email.");
        }

        return cd.IsVerified;
    }
}