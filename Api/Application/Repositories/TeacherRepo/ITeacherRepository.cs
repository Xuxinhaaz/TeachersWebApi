using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.ViewModels.Teachers;
using Api.Models;
using Api.Models.Dtos;

namespace Api.Application.Repositories.TeacherRepo
{
    public interface ITeacherRepository
    {
        Task<List<TeacherModel>> Get(int pageNumber);
        Task<TeacherModel> GetByID(string ID);
        TeacherModel Generate(TeacherViewModel model); 
        List<TeacherDto> MapEntities(List<TeacherModel> teacherModels);
        TeacherDto MapEntity(TeacherModel teacherModel);
    }
}