using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.Models;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;

namespace MyContosoUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Instructor/
        public ActionResult Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department)) // loads Courses, and for each Course that is loaded it does eager loading for the Course.Department navigation property.
                .OrderBy(i => i.LastName);
            /*
             * When you retrieved the list of instructors, you specified eager loading for the Courses navigation property and for the Department 
             * property of each course. Then you put the Courses collection in the view model, and now you're accessing the Enrollments navigation
             * property from one entity in that collection. Because you didn't specify eager loading for the Course.Enrollments navigation property,
             * the data from that property is appearing in the page as a result of lazy loading.
             */

            if (id != null)
            {
                /*
                 If an instructor ID was selected, the selected instructor is retrieved from the list of instructors in the view model. 
                 * The view model's Courses property is then loaded with the Course entities from that instructor's Courses navigation property.
                 * 
                 */
                ViewBag.InstructorID = id.Value;
                viewModel.Courses = viewModel.Instructors.Where(
                    i => i.InstructorID == id.Value).Single().Courses;
                /*
                 You use the Single method on a collection when you know the collection will have only one item. The Single method throws
                 * an exception if the collection passed to it is empty or if there's more than one item. An alternative is SingleOrDefault,
                 * which returns a default value (null in this case) if the collection is empty. However, in this case that would still result
                 * in an exception (from trying to find a Courses property on a null reference), and the exception message would less clearly 
                 * indicate the cause of the problem.
                 */
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                // lazy loading
                //viewModel.Enrollments = viewModel.Courses.Where(
                //    x => x.CourseID == courseID).Single().Enrollments;

                // example of explicit loading of the Enrollments property.
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
                db.Entry(selectedCourse).Collection(x => x.Enrollments).Load(); //  explicitly loads that course's Enrollments navigation property:
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    db.Entry(enrollment).Reference(x => x.Student).Load(); // explicitly loads each Enrollment entity's related Student entity:
                }
                /* Notice that you use the Collection method to load a collection property, but for a property that holds just one entity, you use the Reference method. */

                viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }

        //
        // GET: /Instructor/Details/5

        public ActionResult Details(int id = 0)
        {
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        //
        // GET: /Instructor/Create

        public ActionResult Create()
        {
            ViewBag.InstructorID = new SelectList(db.OfficeAssignments, "InstructorID", "Location");
            return View();
        }

        //
        // POST: /Instructor/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InstructorID = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.InstructorID);
            return View(instructor);
        }

        //
        // GET: /Instructor/Edit/5

        public ActionResult Edit(int id = 0)
        {
            /**
             * When you edit an instructor record, you want to be able to update the instructor's office assignment. The Instructor entity has 
             * a one-to-zero-or-one relationship with the OfficeAssignment entity, which means you must handle the following situations:
             * - If the user clears the office assignment and it originally had a value, you must remove and delete the OfficeAssignment entity.
             * - If the user enters an office assignment value and it originally was empty, you must create a new OfficeAssignment entity.
             * - If the user changes the value of an office assignment, you must change the value in an existing OfficeAssignment entity.
             * 
             * 
             * 
             * ***********/
            // The scaffolded code here isn't what you want. It's setting up data for a drop-down list, but you what you need is a text box
            //Instructor instructor = db.Instructors.Find(id);
            //if (instructor == null)
            //{
            //    return HttpNotFound();
            //}
            
            //ViewBag.InstructorID = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.InstructorID);


            // eager loading for the associated OfficeAssignment entity. You can't perform eager loading with the Find method, 
            // so the Where and Single methods are used instead to select the instructor.
            Instructor instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.InstructorID == id)
                .Single();
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        //
        // POST: /Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection, string[] selectedCourses)
        {
            // Gets the current Instructor entity from the database using eager loading for the OfficeAssignment navigation property.
            var instructorToUpdate = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.InstructorID == id)
                .Single();

            // Updates the retrieved Instructor entity with values from the model binder. The TryUpdateModel overload used
            // enables you to whitelist the properties you want to include
            if (TryUpdateModel(instructorToUpdate, "",
               new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                    {
                        // If the office location is blank, sets the Instructor.OfficeAssignment property to null so that the related row in the OfficeAssignment table will be deleted.
                        instructorToUpdate.OfficeAssignment = null;
                    }

                    /* Since the view doesn't have a collection of Course entities, the model binder can't automatically update the Courses 
                     * navigation property. Instead of using the model binder to update the Courses navigation property, you'll do that in 
                     * the new UpdateInstructorCourses method. Therefore you need to exclude the Courses property from model binding. 
                     * This doesn't require any change to the code that calls TryUpdateModel because you're using the whitelisting overload
                     * and Courses isn't in the include list.
                     */
                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                    db.Entry(instructorToUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            //ViewBag.InstructorID = new SelectList(db.OfficeAssignments, "InstructorID", "Location", id);
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        //
        // GET: /Instructor/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        //
        // POST: /Instructor/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = db.Instructors
                     .Include(i => i.OfficeAssignment)
                     .Where(i => i.InstructorID == id)
                     .Single();

            instructor.OfficeAssignment = null;
            db.Instructors.Remove(instructor);

            /*
             *  If you try to delete an instructor who is assigned to a department as administrator, you'll get a referential integrity error.
                From EF6 using MVC5 Sample
             *  var department = db.Departments
                        .Where(d => d.InstructorID == id)
                        .SingleOrDefault();
                    if (department != null)
                    {
                        department.InstructorID = null;
                    }
             */
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = db.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.CourseID));
            foreach (var course in db.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString())) 
                { 
                    if (!instructorCourses.Contains(course.CourseID))
                    { // If the check box for a course was selected but the course isn't in the Instructor.Courses navigation property
                        instructorToUpdate.Courses.Add(course);
                    }
                }
                else 
                {
                    if (instructorCourses.Contains(course.CourseID))
                    { // If the check box for a course wasn't selected, but the course is in the Instructor.Courses navigation property
                        instructorToUpdate.Courses.Remove(course);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}