using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Application.ViewModels.Teachers
{
    public class TeacherViewModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string singleProperty { get; set; }
        public string IsTeacher = "true";
    }
}