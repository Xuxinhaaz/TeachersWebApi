using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Dtos
{
    public class TeachersProfileDto : IModel
    {
        public string Bio { get; set; }
        public string Description { get; set; }
        public string TeachersTraining { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}