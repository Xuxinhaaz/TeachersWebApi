using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels.Teachers;
using Api.Models.Dtos;
using FluentValidation;

namespace Api.Services.ModelsValidator
{
    public class TeacherValidator : AbstractValidator<TeacherViewModel>
    {
        public TeacherValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must have at least 3 characters.")
                .MaximumLength(30).WithMessage("Name must have at most 30 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must have at least 8 characters.")
                .MaximumLength(30).WithMessage("Password must have at most 30 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MinimumLength(6).WithMessage("Email must have at least 6 characters.")
                .MaximumLength(50).WithMessage("Email must have at most 50 characters.");
        }
    }
}