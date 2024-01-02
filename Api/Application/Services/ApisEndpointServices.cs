using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.TeachersRelation;
using Api.Models.UsersRelation;

namespace Api.Services
{
    public class ApisEndpointServices
    {
        public static async Task<IResult> BadRequestWithMessage(dynamic message, Dictionary<string, dynamic> errors)
        {
            errors["Errors"] = message;

            return Results.BadRequest(errors);
        }

        public static UserModel GenerateUser(UserDto dto)
        {
            return new UserModel(){
                Email = dto.Email,
                IsTeacher = "false",
                UsersID = Guid.NewGuid().ToString(),
                Name = dto.Name
            };
        }    

        public static UsersProfile GenerateUsersProfile(UsersProfileDto dto, UserModel user)
        {
            return new UsersProfile{
                ProfileID = user.UsersID,
                UsersProfileID = Guid.NewGuid().ToString(),
                UserName = user.Name,
                Bio = dto.Bio,
                Description = dto.Description,
                Classroom = dto.Classroom
            };
        } 

        public static TeacherModel GenerateTeacher(TeacherDto dto)
        {
            var newTeacher = new TeacherModel()
            {
                Name = dto.Name,
                TeachersID = Guid.NewGuid().ToString(),
                Email = dto.Email,
                SingleProperty = dto.singleProperty,
                HashedPassword = dto.Password
            };

            return newTeacher;
        }

        public static TeachersProfile GenerateTeachersProfile(TeachersProfileDto dto, TeacherModel teacher)
        {
            var teachersProfile = new TeachersProfile()
            {
                ProfileID = Guid.NewGuid().ToString(),
                TeachersProfileID = teacher.TeachersID,
                Bio = dto.Bio,
                Description = dto.Description,
                TeachersName = dto.Name,
                TeachersTraining = dto.Email
            };

            return teachersProfile;
        }
    }
}