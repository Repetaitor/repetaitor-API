using Core.Application.Interfaces.Repositories;
using Core.Domain.Data;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;

namespace Core.Domain.Repositories;

public class AuthCodesRepository(ApplicationContext context) : IAuthCodesRepository
{
    private ApplicationContext _context = context;

    public async Task<string> CreateAuthCode(string code, string email)
    {
        try
        {
            var newGuid = Guid.NewGuid();
            var auth = new AuthenticationCodes()
            {
                Code = code,
                Guid = newGuid.ToString(),
                Email = email,
                IsVerified = false
            };
            await _context.AuthCodes.AddAsync(auth);
            await _context.SaveChangesAsync();
            return newGuid.ToString();
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public async Task<bool> CheckAuthCode(string guid, string email, string code)
    {
        try
        {
            var cd = await _context.AuthCodes.FirstOrDefaultAsync(x => x.Guid == guid && x.Email == email && x.IsVerified == false);
            if (cd == null || cd.Email != email || cd.Code != code || (DateTime.UtcNow - cd.CreateDate).TotalMinutes > 10) {return false;}
            cd.IsVerified = true;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> EmailIsVerfied(string guid, string email)
    {
        try
        {
            var cd = await _context.AuthCodes.FirstOrDefaultAsync(x => x.Email == email && x.Guid == guid);
            return cd is { IsVerified: true };
        }
        catch (Exception)
        {
            return false;
        }
    }
}