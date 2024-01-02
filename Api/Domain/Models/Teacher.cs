using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class TeacherModel : IModel
    {
        [Key]
        public string? TeachersID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? HashedPassword { get; set; }
        public string? SingleProperty { get; set; }
        public string IsTeacher = "true";
    }
}