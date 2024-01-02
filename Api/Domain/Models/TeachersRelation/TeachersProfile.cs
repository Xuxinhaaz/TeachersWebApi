using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.TeachersRelation
{
    public class TeachersProfile
    {
        [Key]
        public string? ProfileID { get; set; }
        public string? TeachersProfileID { get; set; }
        public string? TeachersName { get; set; }
        public string? Description { get; set; }
        public string? Bio { get; set; }
        public string? TeachersTraining { get; set; }

    }
}