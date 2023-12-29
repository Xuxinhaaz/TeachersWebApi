using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.UsersRelation;
using Api.Services;
using BCrypt.Net;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

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

        public async Task<IResult> PostNewUser(UserDto dto)
        {
            var result = ModelValidator.ValidateUserModel(dto);

            var Erros = new Dictionary<string, List<ValidationFailure>>();

            if(!result.IsValid){
                    Erros["errors"] = new List<ValidationFailure>(result.Errors);
                    
                    return Results.BadRequest(Erros);
            }

            var jwtService = new JwtService(Configuration);

            var strToken = jwtService.GenerateUserJwt(dto);

            var newUser = new User(){
                Email = dto.Email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsTeacher = "false",
                UsersID = Guid.NewGuid().ToString(),
                Name = dto.Name
            };

            await Context.Users.AddAsync(newUser);
            await Context.SaveChangesAsync();

            return Results.Ok(new { 
                newUser,
                strToken
            });
        }

        public async Task<IResult> GetUser(string ID)
        {
            var tryFindUser = await Context.Users.AnyAsync(x => x.UsersID == ID);

            if(!tryFindUser)
                return Results.BadRequest("Couldn't find this user");

            var realUser = await Context.Users.FirstAsync(x => x.UsersID == ID);

            return Results.Ok(realUser);
        }

        public async Task<IResult> PostUsersProfile(UsersProfileDto dto, string ID)
        {
            if(string.IsNullOrEmpty(ID))
                return Results.BadRequest("ID must exists!");

            if(dto is null)
                return Results.BadRequest("U must send infos");

            var result = ModelValidator.ValidateUsersProfile(dto);

            var errors = new Dictionary<string, List<ValidationFailure>>();

            if(!result.IsValid)
            {
                errors["Errors"] = new List<ValidationFailure>(result.Errors);

                return Results.BadRequest(errors);
            }

            var anyUser = await Context.Users.AnyAsync(x => x.UsersID == ID);

            if(!anyUser)
                return Results.BadRequest("Couldn't find a user with this ID you provided!");

            var User = await Context.Users.FirstAsync(x => x.UsersID == ID);

            var UsersProfile = new UsersProfile();

            UsersProfile.ProfileID = User.UsersID;
            UsersProfile.UsersProfileID = Guid.NewGuid().ToString();
            UsersProfile.UserName = User.Name;
            UsersProfile.Bio = dto.Bio;
            UsersProfile.Description = dto.Description;
            UsersProfile.Classroom = dto.Classroom;

            await Context.UsersProfiles.AddAsync(UsersProfile);
            await Context.SaveChangesAsync();

            return Results.Ok(UsersProfile);
        }

        public async Task<IResult> DeleteUser(string ID)
        {
            if(string.IsNullOrEmpty(ID))
                return Results.BadRequest("ID must exists!");

            var anyUser = await Context.Users.AnyAsync(x => x.UsersID == ID);
            var anyUsersProfile = await Context.UsersProfiles.AnyAsync(x => x.ProfileID == ID);

            if(!anyUser || !anyUsersProfile)
                return Results.BadRequest("Couldn't find a user with this ID you provided!");

            var User = await Context.Users.FirstAsync(x => x.UsersID == ID);
            var UsersProfile = await Context.UsersProfiles.FirstAsync(x => x.ProfileID == ID);


            Context.Users.Remove(User);
            Context.UsersProfiles.Remove(UsersProfile);
            await Context.SaveChangesAsync();

            return Results.Ok("Deleted!");
        }

    }
}