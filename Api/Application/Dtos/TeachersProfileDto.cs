using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Dtos
{
    public class TeachersProfileDto
    {
        public string Bio { get; set; }
        public string Description { get; set; }
        public string TeachersTraining { get; set; }
        public string TeachersName { get; set; }
    }
}