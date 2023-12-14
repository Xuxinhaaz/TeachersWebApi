using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.UsersRelation
{
    public class UsersProfile
    {
        [Key]
        public string? ProfileID { get; set; }
        public string? UsersProfileID { get; set; }
        public string? UserName { get; set; }
        public string? Description { get; set; }
        public string? Bio { get; set; }
        public string? Classroom { get; set; }
    }
}