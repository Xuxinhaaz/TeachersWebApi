using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Application.Mapping.Users;
using Api.Application.ViewModels;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public UserRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserModel>> Get(int pageNumber)
        {
            return await _context.Users
            .Skip(pageNumber * 5)
            .Take(5)
            .ToListAsync();
        }

        public UserModel Generate(UserViewModel viewModel)
        {
            return new UserModel
            {
                Email = viewModel.Email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(viewModel.Password),
                IsTeacher = "false",
                UsersID = Guid.NewGuid().ToString(),
                Name = viewModel.Name
            };
        }

        public async Task<UserModel> GetByID(string ID)
        {
            return await _context.Users.FindAsync(ID) ?? throw new Exception();
        }

        public async Task<List<UserDto>> MapEntities(List<UserModel> userModels)
        {
            var userDtos = _mapper.Map<List<UserDto>>(userModels);

            return userDtos;
        }

        public async Task<UserDto> MapEntity(UserModel userModel)
        {
            var userDto = _mapper.Map<UserDto>(userModel);

            return userDto;
        }
    }
}