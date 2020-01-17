using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp_Scheduler.Models;

namespace WebApp_Scheduler.Controllers
{
    public class CoursesController : Controller
    {
        private ScheduleContext db = new ScheduleContext();

        // GET: Courses
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.ScheduleType);
            return View(courses.ToList());
        }

        public ActionResult InitialDatabaseEntries()
        {
            var teachingDaysOption = db.TeachingDays.ToList();
            if (teachingDaysOption != null && teachingDaysOption.Count() != 0)
            {
                return RedirectToAction("Index");
            }

            char[] days = new char[] { 'M', 'T', 'W', 'R', 'F' };
            HashSet<string> options = new HashSet<string>();

            for (int i = 0; i < days.Length; i++)
            {
                int counter = 1;


                while (counter <= days.Length)
                {
                    string op = "";
                    for (int k = i; k < counter; k++)
                    {
                        op += days[k];
                    }
                    if (op != "")
                    {
                        options.Add(op);
                    }
                    counter++;

                }

            }


            for (int i = 0; i < days.Length; i++)
            {
                for (int j = 0; j < days.Length; j++)
                {
                    if (i != j)
                    {
                        string opt = "" + days[i] + days[j];
                        options.Add(opt);
                    }
                }
            }
            foreach (var op in options)
            {
                db.TeachingDays.Add(new ScheduleType() { DayOption = op });
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            ViewBag.ScheduleTypeId = new SelectList(db.TeachingDays, "Id", "DayOption");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CourseName,Instructor,ContactHours,HoursPerDay,NumberOfDays,StartDate,EndDate,ScheduleTypeId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ScheduleTypeId = new SelectList(db.TeachingDays, "Id", "DayOption", course.ScheduleTypeId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.ScheduleTypeId = new SelectList(db.TeachingDays, "Id", "DayOption", course.ScheduleTypeId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CourseName,Instructor,ContactHours,HoursPerDay,NumberOfDays,StartDate,EndDate,ScheduleTypeId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ScheduleTypeId = new SelectList(db.TeachingDays, "Id", "DayOption", course.ScheduleTypeId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult AddPrerequisite(int? courseId)
        {
            if (courseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var courses = db.Courses.ToList().Where(x => x.Id != courseId).ToList();
            var alredySelectedCourseIds = db.PrerequisiteCourses.ToList().Where(x => x.ActualCourseId == courseId).ToList().Select(x => x.RequiredCourseId).ToList();
            var selectionOptions = courses.Where(x => alredySelectedCourseIds.Contains(x.Id) == false).ToList().Select(x => new CourseSelectionViewModel() { CourseName = x.CourseName, Id = x.Id }).ToList();
            var alreadySelected = courses.Where(x => alredySelectedCourseIds.Contains(x.Id)).ToList().Select(x => new CourseSelectionViewModel() { CourseName = x.CourseName, Id = x.Id }).ToList();

            ListOfCourseSelectionViewModel listOfCourseSelectionViewModel = new ListOfCourseSelectionViewModel();
            listOfCourseSelectionViewModel.CourseId = Convert.ToInt32(courseId);
            listOfCourseSelectionViewModel.selectedCourses = alreadySelected;
            listOfCourseSelectionViewModel.selectOptions = selectionOptions;

            return View(listOfCourseSelectionViewModel);
        }

        public ActionResult AddPrerequisiteConfirm(int? CourseId, int? Id)
        {

            if (Id == null || CourseId == null)
            {
                return RedirectToAction("AddPrerequisite", new { courseId = CourseId });

            }

            PreRequisite preRequisite = new PreRequisite();
            preRequisite.ActualCourseId = (int)CourseId;
            preRequisite.RequiredCourseId = (int)Id;
            db.PrerequisiteCourses.Add(preRequisite);
            db.SaveChanges();
            return RedirectToAction("AddPrerequisite", new { courseId = CourseId });

        }

        public ActionResult RemovePrerequisite(int? CourseId, int? Id)
        {
            if (Id == null && CourseId == null)
            {
                return RedirectToAction("AddPrerequisite", new { courseId = CourseId });

            }
            PreRequisite preRequisite = db.PrerequisiteCourses.Where(x => x.ActualCourseId == CourseId && x.RequiredCourseId == Id).FirstOrDefault();
            if (preRequisite != null)
            {
                db.PrerequisiteCourses.Remove(preRequisite);
                db.SaveChanges();

            }
            return RedirectToAction("AddPrerequisite", new { courseId = CourseId });

        }

        public ActionResult DisplayPrerequsite(int? courseId)
        {
            if (courseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var allRequiredCourseIds = db.PrerequisiteCourses.ToList().Where(x => x.ActualCourseId == courseId).Select(x => x.RequiredCourseId).ToList();
            List<string> courses = db.Courses.ToList().Where(x => allRequiredCourseIds.Contains(x.Id)).Select(x => x.CourseName).ToList();
            return View(courses);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
