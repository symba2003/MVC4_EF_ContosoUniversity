﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Instructor : Person
    {
        //public int PersonID { get; set; }

        ////[Required]
        //[Display(Name = "Last Name")]
        //[StringLength(50, MinimumLength=1)]
        //public string LastName { get; set; }

        //[Required]
        //[Column("FirstName")]
        //[Display(Name = "First Name")]
        //[StringLength(50)]
        //public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        // FullName is a calculated property that returns a value that's created by concatenating two other properties. Therefore it has only a get accessor, and no FullName column will be generated in the database.
        //public string FullName
        //{
        //    get { return LastName + ", " + FirstMidName; }
        //}

        public virtual ICollection<Course> Courses { get; set; }
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}