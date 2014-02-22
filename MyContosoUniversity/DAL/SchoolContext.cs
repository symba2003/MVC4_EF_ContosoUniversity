using ContosoUniversity.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ContosoUniversity.DAL
{
    public class SchoolContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // If you didn't do this, the generated tables would be named Students, Courses, and Enrollments. Instead, the table names will be Student, Course, and Enrollment.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // For the many-to-many relationship between the Instructor and Course entities, the code specifies the table and column names for the join table. Code First can configure the many-to-many relationship for you without this code, but if you don't call it, you will get default names such as InstructorInstructorID for the PersonID column.
            modelBuilder.Entity<Course>()
             .HasMany(c => c.Instructors).WithMany(i => i.Courses)
             .Map(t => t.MapLeftKey("CourseID")
                 .MapRightKey("PersonID")
                 .ToTable("CourseInstructor"));

            /**
             The following code provides an example of how you could have used fluent API instead of attributes to specify the relationship between 
             * the Instructor and OfficeAssignment entities: 
                modelBuilder.Entity<Instructor>()
                    .HasOptional(p => p.OfficeAssignment).WithRequired(p => p.Instructor);
             */
        }
    }
}