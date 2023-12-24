using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Services.ModelsValidator;
using Api.Services.ModelsValidator.TeachersRelationValidator;
using Api.Services.ModelsValidator.UsersRelationValidator;
using Api.Services.Validator;
using FluentValidation;
using FluentValidation.Results;

namespace Api.Services
{
    public static class ModelValidator
    {
        public static ValidationResult ValidateUserModel(UserDto dto)
        {
            var validator = new UserValidator();

            ValidationResult result = validator.Validate(dto);
            
            return result;
        }

        public static ValidationResult ValidateUsersProfile(UsersProfileDto dto)
        {
            var validor = new UsersProfileValidator();

            ValidationResult result = validor.Validate(dto);

            return result;
        }

        public static ValidationResult ValidateTeacherModel(TeacherDto dto)
        {
            var validator = new TeacherValidator();

            ValidationResult result = validator.Validate(dto);

            return result;
        }

        public static ValidationResult ValidateTeachersProfile(TeachersProfileDto dto)
        {
            var validator = new TeachersProfileValidator();

            var result = validator.Validate(dto);

            return result;
        }
    }
}