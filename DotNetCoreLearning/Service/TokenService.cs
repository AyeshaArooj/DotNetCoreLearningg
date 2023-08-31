namespace DotNetCoreLearning.Service
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using DotNetCoreLearning.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    //using System;
    //using System.IdentityModel.Tokens.Jwt;
    //using System.Security.Claims;
    //using Microsoft.Extensions.Configuration;
    //using Microsoft.IdentityModel.Tokens;

    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;

        public TokenService(IConfiguration configuration, Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public string GenerateToken(string userName, IList<string> userRoles)
        {
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,userName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };


            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                 authClaims,
                 expires: DateTime.Now.AddHours(3),               
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
