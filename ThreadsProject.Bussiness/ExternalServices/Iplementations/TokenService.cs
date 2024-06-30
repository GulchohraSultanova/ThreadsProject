using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Bussiness.ExternalServices.Interfaces;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.ExternalServices.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public TokenResponseDto CreateToken(User user, int expireDate = 180)
        {
            List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Surname, user.Surname),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.GivenName, user.Name)
                };

            // Rolleri ekle
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims, notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expireDate),
                signingCredentials: signingCredentials
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(jwtSecurityToken);
            return new()
            {
                Token = token,
                ExpireDate = jwtSecurityToken.ValidTo,
                UserName = user.UserName,
            };
        }
    }
}
