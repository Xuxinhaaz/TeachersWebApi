using System.Text;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Api.Models;
using Api.Models.Dtos;
using Api.Application.ViewModels;
using Api.Application.ViewModels.Teachers;

namespace Api.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration configuration;
        public JwtService(IConfiguration _Config)
        {
            configuration = _Config;
        }

        public string GenerateUserJwt(UserViewModel viewModel)
        {
            var handler = new JwtSecurityTokenHandler();

            var JwtKey = Encoding.ASCII.GetBytes(configuration["Jwt"]);

            var signinCredentials = new SigningCredentials(
                new SymmetricSecurityKey(JwtKey), 
                SecurityAlgorithms.HmacSha256Signature
            );

            var Claims = new ClaimsIdentity(new List<Claim> {
                new Claim(ClaimTypes.Name, viewModel.Name),
                new Claim(ClaimTypes.Email, viewModel.Email),
                new Claim("IsTeacher", viewModel.IsTeacher)
            });

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = Claims,
                SigningCredentials = signinCredentials,
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = "http://localhost:5224",
                Audience = "http://localhost:5224"
            };

            var token = handler.CreateToken(tokenDescriptor);

            var stringToken = handler.WriteToken(token);

            return stringToken;
        }

        public string GenerateTeachersJwt(TeacherViewModel model)
        {
            var handler = new JwtSecurityTokenHandler();

            var JwtKey = Encoding.ASCII.GetBytes(configuration["Jwt"]);

            var signinCredentials = new SigningCredentials(
                new SymmetricSecurityKey(JwtKey), 
                SecurityAlgorithms.HmacSha256Signature
            );

            var Claims = new ClaimsIdentity(new List<Claim> {
                new Claim(ClaimTypes.Name, model.Name),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim("IsTeacher", model.IsTeacher)
            });

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = Claims,
                SigningCredentials = signinCredentials,
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = "http://localhost:5224",
                Audience = "http://localhost:5224"
            };

            var token = handler.CreateToken(tokenDescriptor);

            var stringToken = handler.WriteToken(token);

            return stringToken;
        }

        public async Task<bool> ValidateUsersJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var JwtKey = Encoding.ASCII.GetBytes(configuration["Jwt"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "http://localhost:5224",
                ValidAudience = "http://localhost:5224",
                IssuerSigningKey = new SymmetricSecurityKey(JwtKey)
            };

            ClaimsPrincipal principal = handler.ValidateToken(token, validationParameters, out _);

            var isTeacherClaim = principal.FindFirst("IsTeacher")?.Value;

            return !string.IsNullOrEmpty(isTeacherClaim) && isTeacherClaim.ToLower() == "false";
        }

        public async Task<bool> ValidateTeachersJwt(string token)
        {
            var hanlder = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters(){
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "http://localhost:5224",
                ValidAudience = "http://localhost:5224",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt"]))
            };

            ClaimsPrincipal response = hanlder.ValidateToken(token, validationParameters, out _);

            var isTeacherClaim = response.FindFirst("IsTeacher")?.Value;
            
            var isExpired = response.HasClaim(c => c.Type == ClaimTypes.Expiration && DateTime.Parse(c.Value) <= DateTime.UtcNow);

            return !string.IsNullOrEmpty(isTeacherClaim) && isTeacherClaim.ToLower() == "true" && !isExpired; 
        }
    }
}