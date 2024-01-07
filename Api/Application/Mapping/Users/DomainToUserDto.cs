using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Dtos;
using AutoMapper;

namespace Api.Application.Mapping.Users
{
    public class DomainToUserDto : Profile
    {
        public DomainToUserDto()
        {
            CreateMap<UserModel, UserDto>();
        }
    }
}