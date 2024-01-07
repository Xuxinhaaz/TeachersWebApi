using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Models.TeachersRelation;
using AutoMapper;

namespace Api.Application.Mapping.Teachers
{
    public class DomainToTeachersProfileDto : Profile
    {
        public DomainToTeachersProfileDto()
        {
            CreateMap<TeachersProfile, TeachersProfileDto>();
        }
    }
}