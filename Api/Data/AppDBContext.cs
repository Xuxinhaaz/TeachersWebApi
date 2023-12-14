using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.TeachersRelation;
using Api.Models.UsersRelation;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeachersProfile> TeachersProfiles { get; set; }
        public DbSet<UsersProfile> UsersProfiles { get; set; }
    }
}