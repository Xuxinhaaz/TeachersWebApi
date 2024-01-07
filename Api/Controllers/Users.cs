using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Repositories;
using Api.Application.Repositories.User;
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
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _UsersRepo;
        private readonly UsersProfileRepository _UsersProfileRepo;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;


        public UsersController(
            AppDBContext context, 
            ConfigurationManager configuration, 
            IUserRepository userRepository, 
            UsersProfileRepository usersProfileRepository,
            IJwtService jwtService)
        {
            _context = context;
            _configuration = configuration;
            _UsersRepo = userRepository;
            _UsersProfileRepo = usersProfileRepository;
            _jwtService = jwtService;
        }

        public async Task<IResult> PostNewUser(UserViewModel viewModel)
        {
            var result = ModelValidator.ValidateUserModel(viewModel);
            var errors = new Dictionary<string, dynamic>();

            if(!result.IsValid)
                return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);

            var Token = _jwtService.GenerateUserJwt(viewModel);

            var UserModel = _UsersRepo.Generate(viewModel);

            await _context.Users.AddAsync(UserModel);
            await _context.SaveChangesAsync();

            var User = _UsersRepo.MapEntity(UserModel);

            return Results.Ok(new { 
                User,
                Token
            });
        }

        public async Task<IResult> GetUserByID(string ID, string Authorization)
        {
            var errors = new Dictionary<string, dynamic>();
            
            var strToken = Authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateUsersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();

            var tryFindUser = await _context.Users.AnyAsync(x => x.UsersID == ID);

            if(!tryFindUser)
                return await ApisEndpointServices.BadRequestWithMessage("User Not Found.", errors);

            var User = await _UsersRepo.GetByID(ID);

            return Results.Ok(new {
                User
            });
        }

        public async Task<IResult> GetUsers(int pageNumber, string Authorization)
        {
            var strToken = Authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateUsersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();
            
            var UsersModel = await _UsersRepo.Get(pageNumber);

            var UsersDto = await _UsersRepo.MapEntities(UsersModel);

            return Results.Ok(new { 
                Users = UsersDto
            });
        }

        public async Task<IResult> PostUsersProfile(UsersProfileViewModel viewModel, string ID, string Authorization)
        {
            var errors = new Dictionary<string, dynamic>();
            var result = ModelValidator.ValidateUsersProfile(viewModel);
            
            if(string.IsNullOrEmpty(ID))
                return await ApisEndpointServices.BadRequestWithMessage("Teacher ID must not be empty.", errors);
            
            var strToken = Authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateUsersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();

            if(!result.IsValid)
                return await ApisEndpointServices.BadRequestWithMessage(result.Errors.ToList(), errors);

            var anyUser = await _context.Users.AnyAsync(x => x.UsersID == ID);
            if(!anyUser)
                return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided ID.", errors);
            var User = await _context.Users.FirstAsync(x => x.UsersID == ID);

            var UsersProfileModel = await _UsersProfileRepo.Generate(viewModel, ID);

            await _context.UsersProfiles.AddAsync(UsersProfileModel);
            await _context.SaveChangesAsync();

            var UsersProfileDto = _UsersProfileRepo.MapEntity(UsersProfileModel);

            return Results.Ok(new {
                UsersProfile = UsersProfileDto
            });
        }

        public async Task<IResult> DeleteUser(string ID, string Authorization)
        {
            var errors = new Dictionary<string, dynamic>();

            if(string.IsNullOrEmpty(ID))
                return await ApisEndpointServices.BadRequestWithMessage("Teacher ID must not be empty.", errors);

            var strToken = Authorization.Replace("Bearer ", "");
            var responseAuth = await _jwtService.ValidateTeachersJwt(strToken);
            if (!responseAuth)
                return Results.Unauthorized();
            
            var anyUser = await _context.Users.AnyAsync(x => x.UsersID == ID);
            var anyUsersProfile = await _context.UsersProfiles.AnyAsync(x => x.ProfileID == ID);

            if(!anyUser || !anyUsersProfile)
                return await ApisEndpointServices.BadRequestWithMessage("No teacher found with the provided ID.", errors);

            var User = await _context.Users.FirstAsync(x => x.UsersID == ID);
            var UsersProfile = await _context.UsersProfiles.FirstAsync(x => x.ProfileID == ID);

            _context.Users.Remove(User);
            _context.UsersProfiles.Remove(UsersProfile);
            await _context.SaveChangesAsync();

            return Results.Ok(new {
                Message = "Deleted!"
            });
        }

        public async Task<IResult> DeleteMax()
        {
            _context.Users.RemoveRange(await _context.Users.ToListAsync());
            await _context.SaveChangesAsync();

            return Results.Ok();
        }

    }
}