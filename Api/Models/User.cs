using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class User
    {
        [Key]
        public string? UsersID { get; set; }
        public string? UserName { get; set; }
        public string? HashedPassword { get; set; }
        public string? UserEmail { get; set; }
    }
}