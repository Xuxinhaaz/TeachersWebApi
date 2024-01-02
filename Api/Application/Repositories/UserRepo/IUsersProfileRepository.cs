using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels.Users;
using Api.Models.Dtos;
using Api.Models.UsersRelation;

namespace Api.Application.Repositories.UserRepo
{
    public interface IUsersProfileRepository
    {
        Task<List<UsersProfileDto>> Get(int pageNumber);
        Task<UserDto> GetByID(string ID);
        Task<UsersProfile> Generate(UsersProfileViewModel viewModel, string ID);
    }
}