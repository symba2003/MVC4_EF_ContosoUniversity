using System;
using System.Collections.Generic;

namespace ContosoUniversity.Models
{
    public class Student
    {
        // The StudentID property will become the primary key column of the database table that corresponds to this class. By default, the Entity Framework interprets a property that's named ID or classnameID as the primary key.
        public int StudentID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        // The Enrollments property is a navigation property. Navigation properties hold other entities that are related to this entity. In this case, the Enrollments property of a Student entity will hold all of the Enrollment entities that are related to that Student entity. 

        // Navigation properties are typically defined as virtual so that they can take advantage of certain Entity Framework functionality such as lazy loading. (Lazy loading will be explained later, in the Reading Related Data tutorial later in this series.
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}