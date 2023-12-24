using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using FluentValidation;

namespace Api.Services.ModelsValidator.TeachersRelationValidator
{
    public class TeachersProfileValidator : AbstractValidator<TeachersProfileDto>
    {
        public TeachersProfileValidator()
        {
            RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must have at least 3 characters!");
            RuleFor(x => x.Name).MaximumLength(30).WithMessage("Name must have at most 30 characters!");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must not be empty!");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid!");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must not be empty!");
            RuleFor(x => x.Email).MinimumLength(6).WithMessage("Email must have at least 6 characters!");
            RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email must have at most 50 charaters!");  
            RuleFor(x => x.TeachersTraining).NotEmpty().WithMessage("Your training must not be empty!");
            RuleFor(x => x.Bio).MinimumLength(10).WithMessage("Bio must have at least 10 characters!");
            RuleFor(x => x.Bio).MaximumLength(100).WithMessage("Bio must have at most 100 characters!");
            RuleFor(x => x.Bio).NotEmpty().WithMessage("Bio must not be empty!");
            RuleFor(x => x.Description).MinimumLength(30).WithMessage("Description must have at least 30 characters!");
            RuleFor(x => x.Description).MaximumLength(300).WithMessage("Description must have at most 300 characters!");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description must not be empty!");
        }
    }
}