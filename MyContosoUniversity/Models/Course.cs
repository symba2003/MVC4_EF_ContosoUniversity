using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        // Basically, this attribute lets you enter the primary key for the course rather than having the database generate it.
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        /*
         The course entity has a foreign key property DepartmentID which points to the related Department entity and it has a Department
         * navigation property. The Entity Framework doesn't require you to add a foreign key property to your data model when you have
         * a navigation property for a related entity.  EF automatically creates foreign keys in the database wherever they are needed.
         * But having the foreign key in the data model can make updates simpler and more efficient. For example, when you fetch a course 
         * entity to edit, the  Department entity is null if you don't load it, so when you update the course entity, you would have to first
         * fetch the  Department entity. When the foreign key property DepartmentID is included in the data model, you don't need to fetch the
         * Department entity before you update.
         */
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Instructor> Instructors { get; set; }
    }
}