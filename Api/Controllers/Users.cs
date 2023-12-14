using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using Api.Services;
using BCrypt.Net;
using FluentValidation.Results;

namespace Api.Controllers
{
    public class UsersController
    {
        private readonly AppDBContext Context;
        private readonly IConfiguration Configuration;

        public UsersController(AppDBContext _context, IConfiguration configuration)
        {
            Context = _context;
            Configuration = configuration;
        }

        public async Task<IResult> PostNewUser(UserDto dto){
            var result = ModelValidator.ValidateUserModel(dto);

            var Erros = new Dictionary<string, List<ValidationFailure>>();

            if(!result.IsValid){
                    Erros["errors"] = new List<ValidationFailure>(result.Errors);
                    
                    return Results.BadRequest(Erros);
            }

            var jwtService = new JwtService(Configuration);

            var strToken = jwtService.Generate(dto);

            var newUser = new User();

            newUser.UsersID = Guid.NewGuid().ToString();
            newUser.Name = dto.Name;
            newUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            newUser.Email = dto.Email;

            await Context.Users.AddAsync(newUser);
            await Context.SaveChangesAsync();

            return Results.Ok(strToken);
        }

    }
}