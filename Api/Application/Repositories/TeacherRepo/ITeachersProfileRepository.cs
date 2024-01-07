using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels.Teachers;
using Api.Models;
using Api.Models.Dtos;
using Api.Models.TeachersRelation;

namespace Api.Application.Repositories.TeacherRepo
{
    public interface ITeachersProfileRepository
    {
        Task<List<TeachersProfile>> Get(int pageNumber);
        Task<TeachersProfile> GetByID(string ID);
        Task<TeachersProfile> GetByUsersID(string ID);
        TeachersProfile Generate(TeachersProfileViewModel model, TeacherModel teacher); 
        List<TeachersProfileDto> MapEntities(List<TeachersProfile> teacherModels);
        TeachersProfileDto MapEntity(TeachersProfile teacherModel);
    }
}