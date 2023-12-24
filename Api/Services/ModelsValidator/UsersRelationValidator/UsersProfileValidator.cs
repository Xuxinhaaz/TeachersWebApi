using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using FluentValidation;

namespace Api.Services.ModelsValidator.UsersRelationValidator
{
    public class UsersProfileValidator : AbstractValidator<UsersProfileDto>
    {
        public UsersProfileValidator()
        {
            RuleFor(x => x.Bio).NotEmpty().NotNull().WithMessage("Bio must exists!");
            RuleFor(x => x.Bio).MaximumLength(200).WithMessage("Bio must have at most 200 characters!");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Bio must have at most 500 characters!");
            RuleFor(x => x.Description).MinimumLength(10).WithMessage("Bio must have at least 10 characters!");
            RuleFor(x => x.Description).NotEmpty().NotNull().WithMessage("Bio must exists!");
            RuleFor(x => x.Classroom).NotEmpty().NotNull().WithMessage("Classroom must exists!");
            RuleFor(x => x.Classroom).MinimumLength(5).WithMessage("Classroom must have at least 5 characters!");
            RuleFor(x => x.Classroom).MaximumLength(20).WithMessage("Classroom must have at most 20 characters!");
        }
    }
}