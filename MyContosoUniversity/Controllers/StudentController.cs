using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.Models;
using ContosoUniversity.DAL;

namespace MyContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Student/

        public ActionResult Index(string sortOrder)
        {
            string s = Resources.MyResource.AddOnName;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            var students = from s in db.Students
                           select s;
            switch (sortOrder)
            {
                case "Name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "Date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            return View(students.ToList());
        }

        //
        // GET: /Student/Details/5

        public ActionResult Details(int id = 0)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Student/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "LastName, FirstMidName, EnrollmentDate")]
            Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // This code adds the Student entity created by the ASP.NET MVC model binder to the Students entity set and then saves the changes to the database. (Model binder refers to the ASP.NET MVC functionality that makes it easier for you to work with data submitted by a form; a model binder converts posted form values to CLR types and passes them to the action method in parameters. In this case, the model binder instantiates a Student entity for you using property values from the Form collection.)
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
           }
           catch (DataException /* dex */)
           {
              //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
              ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
           }

            return View(student);
        }

        //
        // GET: /Student/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
           [Bind(Include = "StudentID, LastName, FirstMidName, EnrollmentDate")]
                Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        //
        // GET: /Student/Delete/5
        /**
         * The method that is called in response to a GET request displays a view that gives the user a chance
         * to approve or cancel the delete operation. If the user approves it, a POST request is created.
         * When that happens, the HttpPost Delete method is called and then that method actually performs
         * the delete operation.
         * ***/
        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Delete/5
        /*
         changed the action method name from DeleteConfirmed to Delete. The scaffolded code named 
         * the HttpPost Delete method DeleteConfirmed to give the HttpPost  method a unique signature. 
         * ( The CLR requires overloaded methods to have different method parameters.) Now that the
         * signatures are unique, you can stick with the MVC convention and use the same name for
         * the HttpPost and HttpGet delete methods.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
                /*
                 If improving performance in a high-volume application is a priority, you could avoid 
                 * an unnecessary SQL query to retrieve the row by replacing the lines of code that call
                 * the Find and Remove methods with the following code as shown in yellow highlight:
                 * 
                 * Student studentToDelete = new Student() { StudentID = id };
                 * db.Entry(studentToDelete).State = EntityState.Deleted;
                 * 
                 */
            }
            catch (DataException/* dex */)
            {
                // uncomment dex and log error. 
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            /**
             To make sure that database connections are properly closed and the resources they hold freed up,
             * you should see to it that the context instance is disposed.
             */
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}