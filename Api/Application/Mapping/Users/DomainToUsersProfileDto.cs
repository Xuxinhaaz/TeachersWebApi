using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Models.UsersRelation;
using AutoMapper;

namespace Api.Application.Mapping.Users
{
    public class DomainToUsersProfileDto : Profile
    {
        public DomainToUsersProfileDto()
        {
            CreateMap<UsersProfile, UsersProfileDto>();
        }
    }
}