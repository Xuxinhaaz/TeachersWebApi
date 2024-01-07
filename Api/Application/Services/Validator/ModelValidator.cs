using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Services.Validator.ModelsValidator.PutValidator;
using Api.Application.ViewModels;
using Api.Application.ViewModels.PutViewModel;
using Api.Application.ViewModels.Teachers;
using Api.Application.ViewModels.Users;
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
        public static ValidationResult ValidateUserModel(UserViewModel viewModel)
        {
            var validator = new UserValidator();

            ValidationResult result = validator.Validate(viewModel);
            
            return result;
        }

        public static ValidationResult ValidateUsersProfile(UsersProfileViewModel viewModel)
        {
            var validor = new UsersProfileValidator();

            ValidationResult result = validor.Validate(viewModel);

            return result;
        }

        public static ValidationResult ValidateTeacherModel(TeacherViewModel viewModel)
        {
            var validator = new TeacherValidator();

            ValidationResult result = validator.Validate(viewModel);

            return result;
        }

        public static ValidationResult ValidateTeachersProfile(TeachersProfileViewModel viewModel)
        {
            var validator = new TeachersProfileValidator();

            var result = validator.Validate(viewModel);

            return result;
        }

        public static ValidationResult ValidatePutTeachersModel(PutTeachersViewModel viewModel)
        {
            var validator = new PutTeachersValidator();

            var result = validator.Validate(viewModel);

            return result;
        }

    }
}