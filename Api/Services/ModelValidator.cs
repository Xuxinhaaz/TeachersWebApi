using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Services.Validator;
using FluentValidation;
using FluentValidation.Results;

namespace Api.Services
{
    public static class ModelValidator
    {
        public static ValidationResult ValidateUserModel(UserDto dto){
            var validator = new UserValidator();

            ValidationResult result = validator.Validate(dto);
            
            return result;
        }
    }
}