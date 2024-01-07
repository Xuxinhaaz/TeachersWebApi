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
             private readonly AppDBContext _context;
             private readonly ITeacherRepository _teachersRepo;
             private readonly ITeachersProfileRepository _teachersProfileRepo;
             public TeachersController(
                 AppDBContext context, 
                 ITeacherRepository teacherRepo,
                 ITeachersProfileRepository teachersProfileRepo,
                 IJwtService jwtService
             )
             {
                 _context = context;
                 _teachersRepo = teacherRepo;
                 _teachersProfileRepo = teachersProfileRepo;
                 _jwtService = jwtService;
             }
     
             public async Task<IResult> GetTeachers(int pageNumber, string authorization)
             {
                 var strToken = authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
                 if (!responseAuth)
                     return Results.Unauthorized();
                 
                 var teacherModels = await _teachersRepo.Get(pageNumber);
                 
                 var teachersDto = _teachersRepo.MapEntities(teacherModels); 
                 return Results.Ok(new {
                     Teachers = teachersDto
                 });
             }
             
             public async Task<IResult> GetTeacherByid(string id, string authorization)
             {
                 var errors = new Dictionary<string, dynamic>();
            
                 var strToken = authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
                 if (!responseAuth)
                     return Results.Unauthorized();

                 var tryFindTeacher = await _context.Teachers.AnyAsync(x => x.TeachersID == id);

                 if(!tryFindTeacher)
                     return await ApisEndpointServices.BadRequestWithMessage("User Not Found.", errors);

                 var teacher = await _teachersRepo.GetByID(id);

                 var teacherDto = _teachersRepo.MapEntity(teacher);

                 return Results.Ok(new {
                     teacher = teacherDto
                 });
             }
     
             public async Task<IResult> PostNewTeacher(TeacherViewModel viewModel)
             {
                 var result = ModelValidator.ValidateTeacherModel(viewModel);
                 var errors = new Dictionary<string, dynamic>(); 
                 
                 if(viewModel is null)
                     return Results.BadRequest("send the data we need to register a teacher!");  
                     
     
                 if(!result.IsValid)
                     return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);
     
                 var strToken = _jwtService.GenerateTeachersJwt(viewModel);
     
                 var newTeacher = _teachersRepo.Generate(viewModel);
     
                 await _context.Teachers.AddAsync(newTeacher);
                 await _context.SaveChangesAsync();
     
                 var teacherDto = _teachersRepo.MapEntity(newTeacher);
     
                 return Results.Ok(new {
                     teacher = teacherDto,
                     Token = strToken
                 });
             }
     
             public async Task<IResult> PostNewTeachersProfile(TeachersProfileViewModel viewModel, string id, string AuthToken)
             {
                 var errors = new Dictionary<string, dynamic>();
                 
                 if(string.IsNullOrEmpty(id))
                     return await ApisEndpointServices.BadRequestWithMessage("teacher id must not be empty.", errors);
     
                 if(string.IsNullOrEmpty(AuthToken))
                     return Results.Unauthorized();
                 
                 var strToken = AuthToken.Replace("Bearer ", "");
                 var responseJwtServiceValidation = await _jwtService.ValidateTeachersJwt(strToken);
                 if(!responseJwtServiceValidation)
                     return Results.Unauthorized();
     
                 var result = ModelValidator.ValidateTeachersProfile(viewModel);
                 if(!result.IsValid)
                     return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);
                 
                 var TeacherChecks = new TeachersCheck(_context);
                 var checksIfTeachersProfileExists = await TeacherChecks.CheckIfExistsTeachersProfile(id);
     
                 if(checksIfTeachersProfileExists)
                     return await ApisEndpointServices.BadRequestWithMessage("No teachers profile found with the provided id.", errors);
                                 
     
                 var anyUserWithProvidedID = await _context.Teachers.AnyAsync(x => x.TeachersID == id);
                 if(!anyUserWithProvidedID)
                     return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided id.", errors);
     
                 var teacher = await _context.Teachers.FirstAsync(x => x.TeachersID == id);
                 
                 var teachersProfile = _teachersProfileRepo.Generate(viewModel, teacher);
                 
                 await _context.TeachersProfiles.AddAsync(teachersProfile);
                 await _context.SaveChangesAsync();
     
                 var teachersProfileDto = _teachersProfileRepo.MapEntity(teachersProfile);
     
                 return Results.Ok(new {
                     teachersProfile = teachersProfileDto
                 });
             }
     
             public async Task<IResult> DeleteATeacher(string id, string authorization)
             {
                 var errors = new Dictionary<string, dynamic>();
     
                 if(string.IsNullOrEmpty(id))
                     return await ApisEndpointServices.BadRequestWithMessage("teacher's id must not be empty!", errors);
                 
                 var strAuth = authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strAuth);
                 if(!responseAuth)
                     return Results.Unauthorized();
     
                 var anyTeacher = await _context.Teachers.AnyAsync(x => x.TeachersID == id);
                 var anyTeachersProfile = await _context.TeachersProfiles.AnyAsync(x => x.TeachersProfileID == id);
                 if(!anyTeacher || !anyTeachersProfile)
                     return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided id.", errors);
     
                 var firstTeacher = await _context.Teachers.FirstAsync(x => x.TeachersID == id);
                 var firstTeachersProfile = await _context.TeachersProfiles.FirstAsync(x => x.TeachersProfileID == id);
     
                 _context.Teachers.Remove(firstTeacher);
                 _context.TeachersProfiles.Remove(firstTeachersProfile);
                 await _context.SaveChangesAsync();
     
                 return Results.Ok(new {
                     message = "Deleted!"
                 });
             }
     
             public async Task<IResult> PutATeacher(string id, string authorization, PutTeachersViewModel model)
             {
                 var teachersCheck = new TeachersCheck(_context);
                 var errors = new Dictionary<string, dynamic>();
                 
                 if(string.IsNullOrEmpty(id))
                     return await ApisEndpointServices.BadRequestWithMessage("No teacher found with provided id!", errors);
                 
                 var responseValidator = ModelValidator.ValidatePutTeachersModel(model);
                 if(!responseValidator.IsValid)
                     return await ApisEndpointServices.BadRequestWithMessage(responseValidator.Errors.ToList(), errors);
     
                 var strToken = authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
                 if(!responseAuth)
                     return Results.Unauthorized();
                 
                 var checkingResult = await teachersCheck.CheckIfExistsTeacher(id);
                 if(!checkingResult)
                     return await ApisEndpointServices.BadRequestWithMessage("No teacher found with provided id!", errors);
     
                 var teacherFound = await _context.Teachers.FirstAsync(x => x.TeachersID == id);
                 teacherFound.Email = model.Email;
                 teacherFound.Name = model.Name;
                 
                 _context.Teachers.Update(teacherFound);
                 await _context.SaveChangesAsync();
     
                 var teacherDto = _teachersRepo.MapEntity(teacherFound);
                 
                 return Results.Ok(new {
                     teacher = teacherDto
                 });
             }

             public async Task<IResult> PutTeachersProfile(string id, string authorization, TeachersProfileViewModel model)
             {
                 var teachersCheck = new TeachersCheck(_context);
                 var errors = new Dictionary<string, dynamic>();
                 var responseValidator = ModelValidator.ValidateTeachersProfile(model);

                 if (string.IsNullOrEmpty(id))
                     return await ApisEndpointServices.BadRequestWithMessage("No Teachers found with provided id", errors);

                 if (!responseValidator.IsValid)
                     return await ApisEndpointServices.BadRequestWithMessage(responseValidator.Errors.ToList(), errors);

                 var strToken = authorization.Replace("Bearer ", "");
                 var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
                 if (!responseAuth)
                     return Results.Unauthorized();

                 var responseChecker = await teachersCheck.CheckIfExistsTeachersAndTeachersProfile(id);
                 if (!responseChecker)
                     return await ApisEndpointServices.BadRequestWithMessage("No teachers found with the provided id.", errors);

                 var teachersProfile = await _context.TeachersProfiles.FirstAsync(x => x.TeachersProfileID == id);
                 teachersProfile.TeachersName = model.Name;
                 teachersProfile.Bio = model.Bio;
                 teachersProfile.TeachersTraining = model.TeachersTraining;
                 teachersProfile.Description = model.Description;

                 await _context.SaveChangesAsync();

                 var teachersProfileDto = _teachersProfileRepo.MapEntity(teachersProfile);

                 return Results.Ok(new
                 {
                     TeachersProfile = teachersProfileDto
                 });
             }
     
         }
}
