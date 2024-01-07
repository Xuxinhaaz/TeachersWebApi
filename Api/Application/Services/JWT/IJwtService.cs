using Api.Application.ViewModels;
using Api.Application.ViewModels.Teachers;

namespace Api.Services;

public interface IJwtService
{
    string GenerateUserJwt(UserViewModel viewModel);
    string GenerateTeachersJwt(TeacherViewModel model);
    Task<bool> ValidateUsersJwt(string token);
    Task<bool> ValidateTeachersJwt(string token);
}