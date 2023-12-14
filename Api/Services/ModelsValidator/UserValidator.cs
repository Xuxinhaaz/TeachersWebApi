using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using FluentValidation;

namespace Api.Services.Validator
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().MinimumLength(3).WithMessage("it must have at least 3 characters");
            RuleFor(x => x.Password).NotEmpty().NotNull().MaximumLength(20).WithMessage("it must have at most 20 characters");
            RuleFor(x => x.Password).NotEmpty().NotNull().MinimumLength(6).WithMessage("it must have at least 6 characters");
            RuleFor(x => x.Password).NotEmpty().NotNull().MaximumLength(40).WithMessage("it must have at most 40 characters");
            RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress().MinimumLength(10).WithMessage("it must have at most 40 characters");
            RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress().MaximumLength(40).WithMessage("it must have at most 40 characters");
        }
    }
}