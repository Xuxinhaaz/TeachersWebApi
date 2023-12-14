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

namespace Api.Services
{
    public class JwtService
    {
        private readonly IConfiguration configuration;
        public JwtService(IConfiguration _Config)
        {
            configuration = _Config;
        }

        public string Generate(IModel model){
            var handler = new JwtSecurityTokenHandler();

            var JwtKey = Encoding.ASCII.GetBytes(configuration["Jwt"]);

            var signinCredentials = new SigningCredentials(
                new SymmetricSecurityKey(JwtKey), 
                SecurityAlgorithms.HmacSha256Signature
            );

            var Claims = new ClaimsIdentity(new List<Claim> {
                new Claim(ClaimTypes.Name, model.Name),
                new Claim(ClaimTypes.Email, model.Email)
            });

            var tokenDescriptor = new SecurityTokenDescriptor(){
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

        public async Task<bool> Validate(string token){
            var hanlder = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters(){
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "http://localhost:5224",
                ValidAudience = "http://localhost:5224",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt"]))
            };

            var response = await hanlder.ValidateTokenAsync(token, validationParameters);

            return response.IsValid;
        }
    }
}