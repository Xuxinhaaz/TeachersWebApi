using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Repositories.UserRepo;
using Api.Application.ViewModels.Users;
using Api.Data;
using Api.Models.Dtos;
using Api.Models.UsersRelation;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Repositories.User
{
    public class UsersProfileRepository : IUsersProfileRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        public UsersProfileRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UsersProfile>> Get(int pageNumber)
        {
            return await _context.UsersProfiles.Skip(pageNumber * 5).Take(5).ToListAsync();
        }

        public async Task<UsersProfile> GetByID(string ID)
        {
            return await _context.UsersProfiles.FirstAsync(x => x.ProfileID == ID);
        }

        public async Task<UsersProfile> GetByUsersID(string UsersID)
        {
            return await _context.UsersProfiles.FirstAsync(x => x.UsersProfileID == UsersID);
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

        public List<UsersProfileDto> MapEntities(List<UsersProfile> usersProfiles)
        {
            var Dtos = _mapper.Map<List<UsersProfileDto>>(usersProfiles);

            return Dtos;
        }

        public UsersProfileDto MapEntity(UsersProfile usersProfile)
        {
            var Dto = _mapper.Map<UsersProfileDto>(usersProfile);

            return Dto;
        }


    }
}