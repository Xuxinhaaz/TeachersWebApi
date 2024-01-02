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

        public async Task<IResult> GetTeachers(int pageNumber)
        {
            var Teachers = await Context.Teachers.Skip(pageNumber * 5).Take(5).ToListAsync();

            return Results.Ok(new {
                Teachers
            });
        }

        public async Task<IResult> PostNewTeacher(TeacherDto dto)
        {
            var result = ModelValidator.ValidateTeacherModel(dto);
            var errors = new Dictionary<string, dynamic>(); 
            var jwtService = new JwtService(Configuration);
            
            if(dto is null)
                return Results.BadRequest("send the data we need to register a teacher!");  
                

            if(!result.IsValid)
                return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);

            var strToken = jwtService.GenerateTeachersJwt(dto);

            var newTeacher = ApisEndpointServices.GenerateTeacher(dto);

            await Context.Teachers.AddAsync(newTeacher);
            await Context.SaveChangesAsync();

            return Results.Ok(new {
                Token = strToken,
                newTeacher.TeachersID
            });
        }

        public async Task<IResult> PostNewTeachersProfile(TeachersProfileDto dto, string ID, string AuthToken)
        {
            var errors = new Dictionary<string, dynamic>();
            var jwtService = new JwtService(Configuration);
            
            if(string.IsNullOrEmpty(ID))
                return await ApisEndpointServices.BadRequestWithMessage("Teacher ID must not be empty.", errors);

            if(string.IsNullOrEmpty(AuthToken))
                return Results.Unauthorized();
            
            var strToken = AuthToken.Replace("Bearer ", "");
            var responseJwtServiceValidation = await jwtService.ValidateUsersJwt(strToken);
            if(!responseJwtServiceValidation)
                return Results.Unauthorized();

            var result = ModelValidator.ValidateTeachersProfile(dto);
            if(!result.IsValid)
                return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);
            
            var TeacherChecks = new TeachersCheck(Context);
            var checksIfTeachersProfileExists = await TeacherChecks.CheckIfExistsTeachersProfile(ID);

            if(checksIfTeachersProfileExists)
                return await ApisEndpointServices.BadRequestWithMessage("No teachers profile found with the provided ID.", errors);
                            

            var anyUserWithProvidedID = await Context.Teachers.AnyAsync(x => x.TeachersID == ID);
            if(!anyUserWithProvidedID)
                return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided ID.", errors);

            var teacher = await Context.Teachers.FirstAsync(x => x.TeachersID == ID);
            
            var teachersProfile = ApisEndpointServices.GenerateTeachersProfile(dto, teacher);
            
            await Context.TeachersProfiles.AddAsync(teachersProfile);
            await Context.SaveChangesAsync();

            return Results.Ok(teachersProfile);
        }

        public async Task<IResult> DeleteATeacher(string ID, string Authorization){
            var errors = new Dictionary<string, dynamic>();
            var jwtService = new JwtService(Configuration);

            if(string.IsNullOrEmpty(ID)){
                errors["Errors"] = "Teacher's ID must not be empty!"; 

                return Results.BadRequest(errors);
            }
            
            var strAuth = Authorization.Replace("Bearer ", "");
            var responseAuth = await jwtService.ValidateTeachersJwt(strAuth);
            if(!responseAuth)
                return Results.Unauthorized();

            var anyTeacher = await Context.Teachers.AnyAsync(x => x.TeachersID == ID);
            var anyTeachersProfile = await Context.TeachersProfiles.AnyAsync(x => x.TeachersProfileID == ID);
            if(!anyTeacher || !anyTeachersProfile){
                errors["Errors"] = "No teacher found with the provided ID.";

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
