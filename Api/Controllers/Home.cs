using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class Home
    {
        private readonly AppDBContext Context;

        public Home(AppDBContext _context)
        {
            Context = _context;
        }

        public IResult OnGetAll()
        {

            return Results.Ok(new List<object>(){  
                Context.Users.ToList(),
                Context.Teachers.ToList(),
                Context.UsersProfiles.ToList(),
                Context.TeachersProfiles.ToList()
            });
        }



    }
}