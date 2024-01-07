using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Repositories.TeacherRepo;
using Api.Application.ViewModels.Teachers;
using Api.Data;
using Api.Models;
using Api.Models.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public TeacherRepository(IMapper mapper, AppDBContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<TeacherModel>> Get(int pageNumber)
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task<TeacherModel> GetByID(string ID)
        {
            return await _context.Teachers.FirstAsync(x => x.TeachersID == ID);
        }

        public TeacherModel Generate(TeacherViewModel model)
        {
            return new TeacherModel{
                Email = model.Email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password),
                IsTeacher = "true",
                Name = model.Name,
                SingleProperty = model.singleProperty,
                TeachersID = Guid.NewGuid().ToString()
            };
        }

        public List<TeacherDto> MapEntities(List<TeacherModel> teacherModels)
        {
            var Dtos = _mapper.Map<List<TeacherDto>>(teacherModels);

            return Dtos;
        }

        public TeacherDto MapEntity(TeacherModel teacherModel)
        {
            var Dto = _mapper.Map<TeacherDto>(teacherModel);

            return Dto;
        }

        
    }
}