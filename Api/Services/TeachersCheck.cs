using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Services
{
    public class TeachersCheck
    {
        private readonly AppDBContext Context;

        public TeachersCheck(AppDBContext context)
        {
            Context = context;
        }

        public async Task<bool> CheckIfExistsTeachersProfile(string ID)
        {
            var anyTeachersProfile = await Context.TeachersProfiles.AnyAsync(x => x.TeachersProfileID == ID);

            return anyTeachersProfile;
        }

        public async Task<bool> CheckIfExistsTeachersAndTeachersProfile(string ID)
        {
            var anyTeachersProfile = await Context.TeachersProfiles.AnyAsync(x => x.TeachersProfileID == ID);
            var anyTeacher = await Context.Teachers.AnyAsync(x => x.TeachersID == ID);

            return anyTeachersProfile && anyTeacher;
        }
    }
}