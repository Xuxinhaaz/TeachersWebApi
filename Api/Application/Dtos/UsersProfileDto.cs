using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Dtos
{
    public class UsersProfileDto
    {
        public string? Description { get; set; }
        public string? Bio { get; set; }
        public string? Classroom { get; set; }
        public string? UserName { get; set; }
    }
}