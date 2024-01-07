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
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;


        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateUserJwt(UserViewModel viewModel)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtKey = Encoding.ASCII.GetBytes(_configuration["Jwt"]);

            var signinCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtKey), 
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
            
            _logger.LogInformation("creating a jwt");

            return stringToken;
        }

        public string GenerateTeachersJwt(TeacherViewModel model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtKey = Encoding.ASCII.GetBytes(_configuration["Jwt"]);

            var signinCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtKey), 
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
            
            _logger.LogInformation("creating a jwt");

            return stringToken;
        }

        public async Task<bool> ValidateUsersJwt(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
    
                var jwtKey = Encoding.ASCII.GetBytes(_configuration["Jwt"]);
    
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:5224",
                    ValidAudience = "http://localhost:5224",
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
                };
                ClaimsPrincipal principal = handler.ValidateToken(token, validationParameters, out _);
                
                var isTeacherClaim = principal.FindFirst("IsTeacher")?.Value;
                
                _logger.LogInformation("checking jwt...");
                
                return !string.IsNullOrEmpty(isTeacherClaim) && isTeacherClaim.ToLower() == "false";
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task<bool> ValidateTeachersJwt(string token)
        {

            try
            {
                var hanlder = new JwtSecurityTokenHandler();
    
                var validationParameters = new TokenValidationParameters(){
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:5224",
                    ValidAudience = "http://localhost:5224",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt"]))
                };

                ClaimsPrincipal response = hanlder.ValidateToken(token, validationParameters, out _);
    
                var isTeacherClaim = response.FindFirst("IsTeacher")?.Value;
                
                var isExpired = response.HasClaim(c => c.Type == ClaimTypes.Expiration && DateTime.Parse(c.Value) <= DateTime.UtcNow);
                
                _logger.LogInformation("checking jwt...");

                return !string.IsNullOrEmpty(isTeacherClaim) && isTeacherClaim.ToLower() == "true" && !isExpired; 
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
    }
}