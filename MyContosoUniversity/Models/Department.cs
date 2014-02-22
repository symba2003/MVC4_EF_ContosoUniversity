using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Administrator")]
        public int? InstructorID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual Instructor Administrator { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

        /*
         Note By convention, the Entity Framework enables cascade delete for non-nullable foreign keys and for many-to-many relationships. 
         * This can result in circular cascade delete rules, which will cause an exception when your initializer code runs. For example,
         * if you didn't define the Department.InstructorID property as nullable, you'd get the following exception message when the initializer
         * runs: "The referential relationship will result in a cyclical reference that's not allowed." If your business rules required InstructorID
         * property as non-nullable, you would have to use the following fluent API to disable cascade delete on the relationship:
         * 
         * modelBuilder.Entity().HasRequired(d => d.Administrator).WithMany().WillCascadeOnDelete(false);
         */
    }
}