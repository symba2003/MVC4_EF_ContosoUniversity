﻿using System.ComponentModel.DataAnnotations;
namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    /**
   There's a many-to-many relationship between the Student and Course entities, and the Enrollment entity functions as a many-to-many join table 
     * with payload in the database. This means that the Enrollment table contains additional data besides foreign keys for the joined tables 
     * (in this case, a primary key and a Grade property).
     * 
     * If the Enrollment table didn't include grade information, it would only need to contain the two foreign keys CourseID and PersonID. 
     * In that case, it would correspond to a many-to-many join table without payload (or a pure join table) in the database, and you wouldn't
     * have to create a model class for it at all. The Instructor and Course entities have that kind of many-to-many relationship, and as you can
     * see, there is no entity class between them. A join table is required in the database, however. The Entity Framework automatically creates
     * the CourseInstructor table, and you read and update it indirectly by reading and updating the Instructor.Courses and Course.Instructors 
     * navigation properties.
     
     */
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int PersonID { get; set; }

        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}