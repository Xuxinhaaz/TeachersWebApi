using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels;
using Api.Models.Dtos;
using FluentValidation;

namespace Api.Services.Validator
{
    public class UserValidator : AbstractValidator<UserViewModel>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull().WithMessage("Name cannot be null.")
                .MinimumLength(3).WithMessage("Name must have at least 3 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .NotNull().WithMessage("Password cannot be null.")
                .MinimumLength(6).WithMessage("Password must have at least 6 characters.")
                .MaximumLength(20).WithMessage("Password must have at most 20 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .NotNull().WithMessage("Email cannot be null.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MinimumLength(10).WithMessage("Email must have at least 10 characters.")
                .MaximumLength(40).WithMessage("Email must have at most 40 characters.");
        }
    }
}