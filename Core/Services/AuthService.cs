using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Services;
public class AuthService(UserManager<AppUser> userManager , IOptions<JwtOptions> options) : IAuthService
{
    public async Task<UserResultDto> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user is null) throw new UnauthorizedAccessException();

        var flag = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!flag) throw new UnauthorizedAccessException();

        return new UserResultDto()
        {
            DisplayName = user.DisplayName,
            Email = user.Email,
            Token = await GenerateJwtTokenAsync(user)
        };
    }

    public async Task<UserResultDto> RegisterAsync(RegisterDto registerDto)
    {
        var user = new AppUser()
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            PhoneNumber = registerDto.PhoneNumber,
        };
        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description);
            var errorMessage = string.Join("; ", errors); // دمج الأخطاء في نص واحد
            throw new ValidationException(errorMessage);
        }

        return new UserResultDto()
        {
            DisplayName = user.DisplayName,
            Email = user.Email,
            Token = await GenerateJwtTokenAsync(user)
        }; 
    }


    private async Task<string> GenerateJwtTokenAsync(AppUser user)
    {

        //Header 
        //Payload
        //Signature

        var jwtOptions = options.Value;

        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name , user.UserName),
            new Claim(ClaimTypes.Email , user.Email),
        };

        var roles = await userManager.GetRolesAsync(user);
        foreach(var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }


        var SecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));


        var token = new JwtSecurityToken(
            issuer : jwtOptions.Issuer,
            audience : jwtOptions.Audience,
            claims : authClaims,
            expires : DateTime.UtcNow.AddDays((jwtOptions.DurationInDays)),
            signingCredentials: new SigningCredentials( SecretKey ,SecurityAlgorithms.HmacSha256Signature)
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}



























//using Domain.Models.Identity;
//using Microsoft.AspNetCore.Identity;
//using Services.Abstractions;
//using Shared;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Services;
//public class AuthService(UserManager<AppUser> userManager) : IAuthService
//{
//    public async Task<UserResultDto> LoginAsync(LoginDto loginDto)
//    {
//        var user = await userManager.FindByEmailAsync(loginDto.Email);
//        if (user is null) throw new UnauthorizedAccessException();

//        var flag =await userManager.CheckPasswordAsync(user , loginDto.Password);
//        if(!flag) throw new UnauthorizedAccessException();

//        return new UserResultDto()
//        {
//            DisplayName = user.DisplayName,
//            Email = user.Email,
//            Token = "TOKEN",
//        };
//    }

//    public async Task<UserResultDto> RegisterAsync(RegisterDto registerDto)
//    {
//        var user = new AppUser()
//        {
//            DisplayName = registerDto.DisplayName,
//            Email = registerDto.Email,
//            UserName = registerDto.UserName,
//            PhoneNumber = registerDto.PhoneNumber,
//        };
//        var result = await userManager.CreateAsync(user , registerDto.Password);
//        if (!result.Succeeded)
//        {
//            var errors = result.Errors.Select(error => error.Description);
//            throw new ValidationException(errors);
//        }

//        return new UserResultDto()
//        {
//            DisplayName = user.DisplayName,
//            Email = user.Email,
//            Token = "TOKEN"
//        };
//    }
//}
