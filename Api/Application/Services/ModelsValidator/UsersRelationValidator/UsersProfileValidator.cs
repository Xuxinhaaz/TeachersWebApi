using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels.Users;
using Api.Models.Dtos;
using FluentValidation;

namespace Api.Services.ModelsValidator.UsersRelationValidator
{
    public class UsersProfileValidator : AbstractValidator<UsersProfileViewModel>
    {
        public UsersProfileValidator()
        {
            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required.")
                .NotNull().WithMessage("Bio cannot be null.")
                .MaximumLength(200).WithMessage("Bio must be at most 200 characters.")
                .MinimumLength(10).WithMessage("Bio must be at least 10 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .NotNull().WithMessage("Description cannot be null.")
                .MaximumLength(500).WithMessage("Description must be at most 500 characters.")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull().WithMessage("Name cannot be null.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .NotNull().WithMessage("Email cannot be null.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MinimumLength(10).WithMessage("Email must be at least 10 characters.")
                .MaximumLength(40).WithMessage("Email must be at most 40 characters.");
        }
    }
}