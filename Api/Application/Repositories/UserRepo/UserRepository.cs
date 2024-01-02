using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;
        public UserRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Get(int pageNumber)
        {
            return await _context.Users
            .Skip(pageNumber * 5)
            .Take(5)
            .Select(x => new UserDto{
                Email = x.Email,
                Name = x.Name,
                UserID = x.UsersID
            }).ToListAsync();
        }

        public UserModel Generate(UserViewModel viewModel)
        {
            return new UserModel{
                Email = viewModel.Email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(viewModel.Password),
                IsTeacher = "false",
                UsersID = Guid.NewGuid().ToString(),
                Name = viewModel.Name
            };
        }

        public async Task<UserDto> GetByID(string ID)
        {
            var user = await _context.Users.FirstAsync(x => x.UsersID == ID);

            return (UserDto) _context
            .Users
            .Select(x => new UserDto{
                Email = user.Email,
                UserID = user.UsersID,
                Name = user.Name
            });
        }
    }
}