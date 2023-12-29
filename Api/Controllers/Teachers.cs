using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.TeachersRelation;
using Api.Services;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace Api.Controllers
{
    public class TeachersController
    {
        private readonly AppDBContext Context;
        private readonly IConfiguration Configuration;
        public TeachersController(AppDBContext _context, IConfiguration _configuration)
        {
            Context = _context;
            Configuration = _configuration;
        }

        public async Task<IResult> PostNewTeacher(TeacherDto dto)
        {
            if(dto is null)
                return Results.BadRequest("send the data we need to register a teacher!");

            var result = ModelValidator.ValidateTeacherModel(dto);
            var errors = new Dictionary<string, List<ValidationFailure>>(); 

            if(!result.IsValid){
                errors["Errors"] = result.Errors.ToList();

                return Results.BadRequest(errors);
            }

            var jwtService = new JwtService(Configuration);
            
            var strToken = jwtService.GenerateTeachersJwt(dto);

            var newTeacher = new Teacher(){
                Name = dto.Name,
                TeachersID = Guid.NewGuid().ToString(),
                Email = dto.Email,
                SingleProperty = dto.singleProperty,
                HashedPassword = dto.Password
            };

            await Context.Teachers.AddAsync(newTeacher);
            await Context.SaveChangesAsync();

            return Results.Ok(new {
                Token = strToken,
                newTeacher.TeachersID
            });
        }

        public async Task<IResult> PostNewTeachersProfile(TeachersProfileDto dto, string ID, string AuthToken)
        {
            if(string.IsNullOrEmpty(ID))
                return Results.BadRequest("Id provided must not be empty");

            if(string.IsNullOrEmpty(AuthToken))
                return Results.Unauthorized();
            
            /* var strToken = AuthToken.Replace("Bearer ", "");
            var jwtService = new JwtService(Configuration);
            var responseJwtServiceValidation = await jwtService.ValidateUsersJwt(strToken);
            if(!responseJwtServiceValidation)
                return Results.Unauthorized(); */

            var result = ModelValidator.ValidateTeachersProfile(dto);
            var errors = new Dictionary<string, dynamic>();
            if(!result.IsValid)
            {
                errors["Errors"] = result.Errors;

                return Results.BadRequest(errors);
            }
            
            var TeacherChecks = new TeachersCheck(Context);
            var checksIfTeachersProfileExists = await TeacherChecks.CheckIfExistsTeachersProfile(ID);

            if(checksIfTeachersProfileExists){
                errors["Errors"] = "TeachersProfile with this ID alredy exists!";

                return Results.BadRequest(errors);
            }
                            

            var anyUserWithProvidedID = await Context.Teachers.AnyAsync(x => x.TeachersID == ID);
            if(!anyUserWithProvidedID)
            {
                errors["Errors"] = "ID provided does not match any teacher!";

                return Results.BadRequest(errors);
            }

            var teacher = await Context.Teachers.FirstAsync(x => x.TeachersID == ID);
            
            var teachersProfile = new TeachersProfile()
            {
                ProfileID = Guid.NewGuid().ToString(),
                TeachersProfileID = teacher.TeachersID,
                Bio = dto.Bio,
                Description = dto.Description,
                TeachersName = dto.Name,
                TeachersTraining = dto.Email
            };

            await Context.TeachersProfiles.AddAsync(teachersProfile);
            await Context.SaveChangesAsync();

            return Results.Ok(teachersProfile);
        }

        public async Task<IResult> DeleteATeacher(string ID, string Authorization){
            var errors = new Dictionary<string, dynamic>();
            var jwtService = new JwtService(Configuration);

            if(string.IsNullOrEmpty(ID)){
                errors["Errors"] = "ID must not be empty!";

                return Results.BadRequest(errors);
            }
            
            var strAuth = Authorization.Replace("Bearer ", "");
            var responseAuth = await jwtService.ValidateTeachersJwt(strAuth);
            if(!responseAuth)
                return Results.Unauthorized();

            var anyTeacher = await Context.Teachers.AnyAsync(x => x.TeachersID == ID);
            var anyTeachersProfile = await Context.TeachersProfiles.AnyAsync(x => x.TeachersProfileID == ID);
            if(!anyTeacher || !anyTeachersProfile){
                errors["Errors"] = "There is no teacher profile with this ID!";

                return Results.BadRequest(errors);
            }

            var firstTeacher = await Context.Teachers.FirstAsync(x => x.TeachersID == ID);
            var firstTeachersProfile = await Context.TeachersProfiles.FirstAsync(x => x.TeachersProfileID == ID);

            Context.Teachers.Remove(firstTeacher);
            Context.TeachersProfiles.Remove(firstTeachersProfile);
            await Context.SaveChangesAsync();

            return Results.Ok("Deleted!");
        }

    }
}
