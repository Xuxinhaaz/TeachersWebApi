using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Teacher
    {
        [Key]
        public string? TeachersID { get; set; }
        public string? TeachersName { get; set; }
        public string? HashedPassword { get; set; }
        public string? SingleProperty { get; set; }

        
    }
}