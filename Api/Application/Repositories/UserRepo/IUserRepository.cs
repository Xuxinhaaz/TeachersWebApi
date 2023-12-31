using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels;
using Api.Models;
using Api.Models.Dtos;

namespace Api.Application.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserModel>> Get(int pageNumber);
        Task<UserModel> GetByID(string ID);
        UserModel Generate(UserViewModel viewModel);
        Task<List<UserDto>> MapEntities(List<UserModel> userModels);
        Task<UserDto> MapEntity(UserModel userModel);
    }
}