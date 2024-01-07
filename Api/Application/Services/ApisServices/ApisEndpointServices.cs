using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.TeachersRelation;
using Api.Models.UsersRelation;

namespace Api.Services
{
    public class ApisEndpointServices
    {
        public static async Task<IResult> BadRequestWithMessage(dynamic message, Dictionary<string, dynamic> errors)
        {
            errors["Errors"] = message;

            return Results.BadRequest(errors);
        }

    }
}