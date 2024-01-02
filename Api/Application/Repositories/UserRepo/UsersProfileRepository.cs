using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Repositories.UserRepo;
using Api.Application.ViewModels.Users;
using Api.Data;
using Api.Models.Dtos;
using Api.Models.UsersRelation;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Repositories.User
{
    public class UsersProfileRepository : IUsersProfileRepository
    {
        private readonly AppDBContext _context;

        public UsersProfileRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<UsersProfileDto>> Get(int pageNumber)
        {
            return await _context.UsersProfiles
            .Skip(pageNumber * 5)
            .Take(5)
            .Select(x => new UsersProfileDto{
                Bio = x.Bio,
                Classroom = x.Classroom,
                Description = x.Description,
                Name = x.UserName
            })
            .ToListAsync();
        }

        public Task<UserDto> GetByID(string ID)
        {
            throw new NotImplementedException();
        }

        public async Task<UsersProfile> Generate(UsersProfileViewModel viewModel, string ID)
        {   
            var FindUser = await _context.Users.FirstAsync(x => x.UsersID == ID);

            return new UsersProfile{
                Bio = viewModel.Bio,
                Classroom = viewModel.Classroom,
                Description = viewModel.Description,
                ProfileID = Guid.NewGuid().ToString(),
                UsersProfileID = FindUser.UsersID,
                UserName = viewModel.Name
            };
        }


    }
}