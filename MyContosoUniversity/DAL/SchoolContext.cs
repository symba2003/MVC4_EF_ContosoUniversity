using ContosoUniversity.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ContosoUniversity.DAL
{
    public class SchoolContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // If you didn't do this, the generated tables would be named Students, Courses, and Enrollments. Instead, the table names will be Student, Course, and Enrollment.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}