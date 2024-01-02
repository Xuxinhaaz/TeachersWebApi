using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Dtos
{
    public class UserDto : IModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? UserID { get; set; }
    }
}