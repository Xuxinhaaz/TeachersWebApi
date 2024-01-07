using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Repositories;
using Api.Application.Repositories.UserRepo;
using Api.Application.ViewModels;
using Api.Application.ViewModels.Users;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.UsersRelation;
using Api.Services;
using AutoMapper;
using BCrypt.Net;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace Api.Controllers
{
    public class UsersController
    {
        private readonly AppDBContext _context;
        private readonly IUserRepository _usersRepository;
        private readonly IUsersProfileRepository _usersProfileRepository;
        private readonly IJwtService _jwtService;


        public UsersController(
            AppDBContext context, 
            IUserRepository userRepository, 
            IUsersProfileRepository usersProfileRepository,
            IJwtService jwtService)
        {
            _context = context;
            _usersRepository = userRepository;
            _usersProfileRepository = usersProfileRepository;
            _jwtService = jwtService;
        }

        public async Task<IResult> PostNewUser(UserViewModel viewModel)
        {
            var result = ModelValidator.ValidateUserModel(viewModel);
            var errors = new Dictionary<string, dynamic>();

            if(!result.IsValid)
                return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);

            var Token = _jwtService.GenerateUserJwt(viewModel);

            var UserModel = _usersRepository.Generate(viewModel);

            await _context.Users.AddAsync(UserModel);
            await _context.SaveChangesAsync();

            var user = await _usersRepository.MapEntity(UserModel);

            return Results.Ok(new { 
                user,
                Token
            });
        }

        public async Task<IResult> GetUserByid(string id, string authorization)
        {
            var errors = new Dictionary<string, dynamic>();
            
            var strToken = authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateUsersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();

            var tryFindUser = await _context.Users.AnyAsync(x => x.UsersID == id);

            if(!tryFindUser)
                return await ApisEndpointServices.BadRequestWithMessage("user Not Found.", errors);

            var user = await _usersRepository.GetByID(id);

            return Results.Ok(new {
                user
            });
        }

        public async Task<IResult> GetUsers(int pageNumber, string authorization)
        {
            var strToken = authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateUsersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();
            
            var userModels = await _usersRepository.Get(pageNumber);

            var userDtos = await _usersRepository.MapEntities(userModels);

            return Results.Ok(new { 
                Users = userDtos
            });
        }

        public async Task<IResult> PostUsersProfile(UsersProfileViewModel viewModel, string id, string authorization)
        {
            var errors = new Dictionary<string, dynamic>();
            var result = ModelValidator.ValidateUsersProfile(viewModel);
            
            if(string.IsNullOrEmpty(id))
                return await ApisEndpointServices.BadRequestWithMessage("Teacher id must not be empty.", errors);
            
            var strToken = authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateUsersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();

            if(!result.IsValid)
                return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);

            var anyUser = await _context.Users.AnyAsync(x => x.UsersID == id);
            if(!anyUser)
                return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided id.", errors);

            var userProfileModel = await _usersProfileRepository.Generate(viewModel, id);

            await _context.UsersProfiles.AddAsync(userProfileModel);
            await _context.SaveChangesAsync();

            var usersProfileDto = _usersProfileRepository.MapEntity(userProfileModel);

            return Results.Ok(new {
                usersProfile = usersProfileDto
            });
        }

        public async Task<IResult> DeleteUser(string id, string authorization)
        {
            var errors = new Dictionary<string, dynamic>();

            if(string.IsNullOrEmpty(id))
                return await ApisEndpointServices.BadRequestWithMessage("Teacher id must not be empty.", errors);

            var strToken = authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();
            
            var anyUser = await _context.Users.AnyAsync(x => x.UsersID == id);
            var anyUsersProfile = await _context.UsersProfiles.AnyAsync(x => x.ProfileID == id);

            if(!anyUser || !anyUsersProfile)
                return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided id.", errors);

            var user = await _context.Users.FirstAsync(x => x.UsersID == id);
            var usersProfile = await _context.UsersProfiles.FirstAsync(x => x.ProfileID == id);

            _context.Users.Remove(user);
            _context.UsersProfiles.Remove(usersProfile);
            await _context.SaveChangesAsync();

            return Results.Ok(new {
                Message = "Deleted!"
            });
        }
        
    }
}