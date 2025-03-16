using Core.Application.Interfaces.Repositories;
using Core.Domain.Data;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;

namespace Core.Domain.Repositories;

public class AuthCodesRepository(ApplicationContext context) : IAuthCodesRepository
{
    private ApplicationContext _context = context;

    public async Task<bool> CreateAuthCode(string code, string email)
    {
        try
        {
            var auth = new AuthenticationCodes()
            {
                Code = code,
                Email = email,
                IsVerified = false
            };
            await _context.AuthCodes.AddAsync(auth);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> CheckAuthCode(string email, string code)
    {
        try
        {
            var cd = await _context.AuthCodes.FirstOrDefaultAsync(x => x.Email == email && x.IsVerified == false);
            if (cd == null || cd.Email != email || cd.Code != code || (DateTime.UtcNow - cd.CreateDate).TotalMinutes > 3) {return false;}
            cd.IsVerified = true;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> EmailIsVerfied(string email)
    {
        try
        {
            var cd = await _context.AuthCodes.OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync(x => x.Email == email);
            return cd != null && cd.IsVerified;
        }
        catch (Exception)
        {
            return false;
        }
    }
}