using System.Security.Claims;
using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Models.RequestsDTO.Authorization;

public class UserSignInResponse
{
    public UserModal User {get; set;}
    public ClaimsIdentity ClaimsIdentity { get; set; }    
}