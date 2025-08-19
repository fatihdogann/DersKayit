using Microsoft.EntityFrameworkCore;
using DersKayit.Models;

namespace DersKayit.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseApplication> CourseApplications { get; set; }
         public DbSet<User> Users { get; set; }
    }
}
