using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using FluentValidation;

namespace Api.Services.ModelsValidator
{
    public class TeacherValidator : AbstractValidator<TeacherDto>
    {
        public TeacherValidator()
        {
            RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must have at least 3 characters!");
            RuleFor(x => x.Name).MaximumLength(30).WithMessage("Name must have at most 30 characters!");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must not be empty!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password must not be empty!");
            RuleFor(x => x.Password).MinimumLength(8).WithMessage("Password must have at least 8 characters!");
            RuleFor(x => x.Password).MaximumLength(30).WithMessage("Password must have at most 30 characters!");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid!");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must not be empty!");
            RuleFor(x => x.Email).MinimumLength(6).WithMessage("Email must have at least 6 characters!");
            RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email must have at most 50 charaters!");        }
    }
}