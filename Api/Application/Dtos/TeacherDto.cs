using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Dtos
{
    public class TeacherDto 
    {
        public string? TeachersID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? SingleProperty { get; set; }
        public string? IsTeacher = "true";
    }
}