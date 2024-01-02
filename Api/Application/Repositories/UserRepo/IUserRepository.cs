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
        Task<List<UserDto>> Get(int pageNumber);
        Task<UserDto> GetByID(string ID);
        UserModel Generate(UserViewModel viewModel);
    }
}