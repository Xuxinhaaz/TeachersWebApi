using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class User : IModel
    {
        [Key]
        public string? UsersID { get; set; }
        public string? Name { get; set; }
        public string? HashedPassword { get; set; }
        public string? Email { get; set; }
        public string? IsTeacher = "false";
    }
}