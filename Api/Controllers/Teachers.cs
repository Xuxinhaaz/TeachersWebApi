using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Repositories;
using Api.Application.Repositories.TeacherRepo;
using Api.Application.ViewModels.PutViewModel;
using Api.Application.ViewModels.Teachers;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.TeachersRelation;
using Api.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace Api.Controllers
{
    public class TeachersController
    {
             private readonly IJwtService _jwtService;
             private readonly AppDBContext Context;
             private readonly IConfiguration Configuration;
             private readonly TeacherRepository _TeachersRepo;
             private readonly TeachersProfileRepository _TeachersProfileRepo;
             public TeachersController(
                 AppDBContext _context, 
                 IConfiguration _configuration, 
                 TeacherRepository teacherRepo,
                 TeachersProfileRepository teachersProfileRepo,
                 IJwtService jwtService
             )
             {
                 Context = _context;
                 Configuration = _configuration;
                 _TeachersRepo = teacherRepo;
                 _TeachersProfileRepo = teachersProfileRepo;
                 _jwtService = jwtService;
             }
     
             public async Task<IResult> GetTeachers(int pageNumber, string Authroization)
             {
                 var strToken = Authroization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
                 if (!responseAuth)
                     return Results.Unauthorized();
                 
                 var TeachersModels = await _TeachersRepo.Get(pageNumber);
                 
                 var teachersDto = _TeachersRepo.MapEntities(TeachersModels); 
                 return Results.Ok(new {
                     Teachers = teachersDto
                 });
             }
     
             public async Task<IResult> PostNewTeacher(TeacherViewModel viewModel)
             {
                 var result = ModelValidator.ValidateTeacherModel(viewModel);
                 var errors = new Dictionary<string, dynamic>(); 
                 var jwtService = new JwtService(Configuration);
                 
                 if(viewModel is null)
                     return Results.BadRequest("send the data we need to register a teacher!");  
                     
     
                 if(!result.IsValid)
                     return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);
     
                 var strToken = jwtService.GenerateTeachersJwt(viewModel);
     
                 var newTeacher = _TeachersRepo.Generate(viewModel);
     
                 await Context.Teachers.AddAsync(newTeacher);
                 await Context.SaveChangesAsync();
     
                 var TeacherDto = _TeachersRepo.MapEntity(newTeacher);
     
                 return Results.Ok(new {
                     Teacher = TeacherDto,
                     Token = strToken
                 });
             }
     
             public async Task<IResult> PostNewTeachersProfile(TeachersProfileViewModel viewModel, string ID, string AuthToken)
             {
                 var errors = new Dictionary<string, dynamic>();
                 
                 if(string.IsNullOrEmpty(ID))
                     return await ApisEndpointServices.BadRequestWithMessage("Teacher ID must not be empty.", errors);
     
                 if(string.IsNullOrEmpty(AuthToken))
                     return Results.Unauthorized();
                 
                 var strToken = AuthToken.Replace("Bearer ", "");
                 var responseJwtServiceValidation = await _jwtService.ValidateTeachersJwt(strToken);
                 if(!responseJwtServiceValidation)
                     return Results.Unauthorized();
     
                 var result = ModelValidator.ValidateTeachersProfile(viewModel);
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
                 
                 var teachersProfile = _TeachersProfileRepo.Generate(viewModel, teacher);
                 
                 await Context.TeachersProfiles.AddAsync(teachersProfile);
                 await Context.SaveChangesAsync();
     
                 var teachersProfileDto = _TeachersProfileRepo.MapEntity(teachersProfile);
     
                 return Results.Ok(new {
                     teachersProfile = teachersProfileDto
                 });
             }
     
             public async Task<IResult> DeleteATeacher(string ID, string Authorization)
             {
                 var errors = new Dictionary<string, dynamic>();
     
                 if(string.IsNullOrEmpty(ID))
                     return await ApisEndpointServices.BadRequestWithMessage("Teacher's ID must not be empty!", errors);
                 
                 var strAuth = Authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strAuth);
                 if(!responseAuth)
                     return Results.Unauthorized();
     
                 var anyTeacher = await Context.Teachers.AnyAsync(x => x.TeachersID == ID);
                 var anyTeachersProfile = await Context.TeachersProfiles.AnyAsync(x => x.TeachersProfileID == ID);
                 if(!anyTeacher || !anyTeachersProfile)
                     return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided ID.", errors);
     
                 var firstTeacher = await Context.Teachers.FirstAsync(x => x.TeachersID == ID);
                 var firstTeachersProfile = await Context.TeachersProfiles.FirstAsync(x => x.TeachersProfileID == ID);
     
                 Context.Teachers.Remove(firstTeacher);
                 Context.TeachersProfiles.Remove(firstTeachersProfile);
                 await Context.SaveChangesAsync();
     
                 return Results.Ok(new {
                     message = "Deleted!"
                 });
             }
     
             public async Task<IResult> PutATeacher(string ID, string Authorization, PutTeachersViewModel model)
             {
                 var TeachersCheck = new TeachersCheck(Context);
                 var errors = new Dictionary<string, dynamic>();
                 
                 if(string.IsNullOrEmpty(ID))
                     return await ApisEndpointServices.BadRequestWithMessage("No Teacher found with provided ID!", errors);
                 
                 var responseValidator = ModelValidator.ValidatePutTeachersModel(model);
                 if(!responseValidator.IsValid)
                     return await ApisEndpointServices.BadRequestWithMessage(responseValidator.Errors.ToList(), errors);
     
                 var strToken = Authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
                 if(!responseAuth)
                     return Results.Unauthorized();
                 
                 var checkingResult = await TeachersCheck.CheckIfExistsTeacher(ID);
                 if(!checkingResult)
                     return await ApisEndpointServices.BadRequestWithMessage("No Teacher found with provided ID!", errors);
     
                 var TeacherFound = await Context.Teachers.FirstAsync(x => x.TeachersID == ID);
                 TeacherFound.Email = model.Email;
                 TeacherFound.Name = model.Name;
                 
                 Context.Teachers.Update(TeacherFound);
                 await Context.SaveChangesAsync();
     
                 var TeacherDto = _TeachersRepo.MapEntity(TeacherFound);
                 
                 return Results.Ok(new {
                     Teacher = TeacherDto
                 });
             }

             public async Task<IResult> PutTeachersProfile(string ID, string Authorization, TeachersProfileViewModel model)
             {
                 var TeachersCheck = new TeachersCheck(Context);
                 var errors = new Dictionary<string, dynamic>();
                 var responseValidator = ModelValidator.ValidateTeachersProfile(model);

                 if (string.IsNullOrEmpty(ID))
                     return await ApisEndpointServices.BadRequestWithMessage("No Teachers found with provided ID", errors);

                 if (!responseValidator.IsValid)
                     return await ApisEndpointServices.BadRequestWithMessage(responseValidator.Errors.ToList(), errors);

                 var strToken = Authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
                 if (!responseAuth)
                     return Results.Unauthorized();

                 var responseChecker = await TeachersCheck.CheckIfExistsTeachersAndTeachersProfile(ID);
                 if (!responseChecker)
                     return await ApisEndpointServices.BadRequestWithMessage("No teachers found with the provided ID.", errors);

                 var teachersProfile = await Context.TeachersProfiles.FirstAsync(x => x.TeachersProfileID == ID);
                 teachersProfile.TeachersName = model.Name;
                 teachersProfile.Bio = model.Bio;
                 teachersProfile.TeachersTraining = model.TeachersTraining;
                 teachersProfile.Description = model.Description;

                 await Context.SaveChangesAsync();

                 var teachersProfileDto = _TeachersProfileRepo.MapEntity(teachersProfile);

                 return Results.Ok(new
                 {
                     TeachersProfile = teachersProfileDto
                 });
             }
     
         }
}
