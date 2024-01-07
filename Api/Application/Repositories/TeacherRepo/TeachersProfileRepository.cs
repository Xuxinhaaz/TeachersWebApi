using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels.Teachers;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.TeachersRelation;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Repositories.TeacherRepo
{
    public class TeachersProfileRepository : ITeachersProfileRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mappper;
        public TeachersProfileRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mappper = mapper;
        }

        public async Task<List<TeachersProfile>> Get(int pageNumber)
        {
            return await _context.TeachersProfiles.Skip(pageNumber * 5).Take(5).ToListAsync();
        }

        public async Task<TeachersProfile> GetByID(string ID)
        {
            return await _context.TeachersProfiles.FirstAsync(x => x.ProfileID == ID);
        }

        public async Task<TeachersProfile> GetByUsersID(string ID)
        {
            return await _context.TeachersProfiles.FirstAsync(x => x.TeachersProfileID == ID);
        }

        public TeachersProfile Generate(TeachersProfileViewModel model, TeacherModel teacher)
        {
            var TeachersProfile = new TeachersProfile{
                Bio = model.Bio,
                Description = model.Description,
                ProfileID = Guid.NewGuid().ToString(),
                TeachersName = model.Name,
                TeachersProfileID = teacher.TeachersID,
                TeachersTraining = model.TeachersTraining
            };

            return TeachersProfile;
        }
        public List<TeachersProfileDto> MapEntities(List<TeachersProfile> teacherModels)
        {
            return _mappper.Map<List<TeachersProfileDto>>(teacherModels);
        }

        public TeachersProfileDto MapEntity(TeachersProfile teacherModel)
        {
            return _mappper.Map<TeachersProfileDto>(teacherModel);
        }

    }
}