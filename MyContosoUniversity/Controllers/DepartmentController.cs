using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.Models;
using ContosoUniversity.DAL;
using System.Data.Entity.Infrastructure;

namespace MyContosoUniversity.Controllers
{
    public class DepartmentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Department/

        public ActionResult Index()
        {
            var departments = db.Departments.Include(d => d.Administrator);
            return View(departments.ToList());
        }

        //
        // GET: /Department/Details/5

        public ActionResult Details(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // GET: /Department/Create

        public ActionResult Create()
        {
            ViewBag.PersonID = new SelectList(db.Instructors, "PersonID", "FullName");
            return View();
        }

        //
        // POST: /Department/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonID = new SelectList(db.Instructors, "PersonID", "FullName", department.PersonID);
            return View(department);
        }

        //
        // GET: /Department/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonID = new SelectList(db.Instructors, "PersonID", "FullName", department.PersonID);
            return View(department);
        }

        //
        // POST: /Department/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
               [Bind(Include = "DepartmentID, Name, Budget, StartDate, RowVersion, PersonID")]
                Department department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                /* The view will store the original RowVersion value in a hidden field. When the model binder creates the department instance, 
                 * that object will have the original RowVersion property value and the new values for the other properties, as entered by the
                 * user on the Edit page. Then when the Entity Framework creates a SQL UPDATE command, that command will include a WHERE clause 
                 * that looks for a row that has the original RowVersion value.
                 * 
                 * If no rows are affected by the UPDATE command (no rows have the original RowVersion value),  the Entity Framework throws
                 * a DbUpdateConcurrencyException exception, and the code in the catch block gets the affected Department entity from the exception
                 * object. This entity has both the values read from the database and the new values entered by the user
                 * */
                var entry = ex.Entries.Single();
                var clientValues = (Department)entry.Entity;

                var databaseValues = (Department)entry.GetDatabaseValues().ToObject();

                if (databaseValues.Name != clientValues.Name)
                    ModelState.AddModelError("Name", "Current value: "
                        + databaseValues.Name);
                if (databaseValues.Budget != clientValues.Budget)
                    ModelState.AddModelError("Budget", "Current value: "
                        + String.Format("{0:c}", databaseValues.Budget));
                if (databaseValues.StartDate != clientValues.StartDate)
                    ModelState.AddModelError("StartDate", "Current value: "
                        + String.Format("{0:d}", databaseValues.StartDate));
                if (databaseValues.PersonID != clientValues.PersonID)
                    ModelState.AddModelError("PersonID", "Current value: "
                        + db.Instructors.Find(databaseValues.PersonID).FullName);
                ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");

                /* Finally, the code sets the RowVersion value of the Department object to the new value retrieved from the database. 
                 * This new RowVersion value will be stored in the hidden field when the Edit page is redisplayed, and the next time the user
                 * clicks Save, only concurrency errors that happen since the redisplay of the Edit page will be caught. */
                department.RowVersion = databaseValues.RowVersion;
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            ViewBag.PersonID = new SelectList(db.Instructors, "PersonID", "FullName", department.PersonID);
            return View(department);
        }

        //
        // GET: /Department/Delete/5
        /* For the Delete page, the Entity Framework detects concurrency conflicts caused by someone else editing the department in a similar manner.
         * When the HttpGet Delete method displays the confirmation view, the view includes the original RowVersion value in a hidden field. 
         * That value is then available to the HttpPost Delete method that's called when the user confirms the deletion. When the Entity Framework 
         * creates the SQL DELETE command, it includes a WHERE clause with the original RowVersion value. If the command results in zero rows 
         * affected (meaning the row was changed after the Delete confirmation page was displayed), a concurrency exception is thrown, and the 
         * HttpGet Delete method is called with an error flag set to true in order to redisplay the confirmation page with an error message. 
         * It's also possible that zero rows were affected because the row was deleted by another user, so in that case a different error message 
         * is displayed.
         */
        public ActionResult Delete(int id, bool? concurrencyError)
        {
            Department department = db.Departments.Find(id);

            if (concurrencyError.GetValueOrDefault())
            {
                if (department == null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                        + "was deleted by another user after you got the original values. "
                        + "Click the Back to List hyperlink.";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                        + "was modified by another user after you got the original values. "
                        + "The delete operation was canceled and the current values in the "
                        + "database have been displayed. If you still want to delete this "
                        + "record, click the Delete button again. Otherwise "
                        + "click the Back to List hyperlink.";
                }
            }

            return View(department);
        }

        //
        // POST: /Department/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Department department)
        {
            // Department parameter instead of id: Department entity instance created by the model binder. This gives you access to the RowVersion property value in addition to the record key.
            try
            {
                db.Entry(department).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}