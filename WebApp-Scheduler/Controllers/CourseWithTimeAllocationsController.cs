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
    public class CourseWithTimeAllocationsController : Controller
    {
        private ScheduleContext db = new ScheduleContext();

        // GET: CourseWithTimeAllocations
        public ActionResult Index()
        {
            var courseWithTimeAllocations = db.CourseWithTimeAllocations.Include(c => c.TimeAllocationHelperInstance);
            return View(courseWithTimeAllocations.ToList());
        }

        // GET: CourseWithTimeAllocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseWithTimeAllocation courseWithTimeAllocation = db.CourseWithTimeAllocations.Find(id);
            if (courseWithTimeAllocation == null)
            {
                return HttpNotFound();
            }
            return View(courseWithTimeAllocation);
        }

        // GET: CourseWithTimeAllocations/Create
        public ActionResult Create()
        {
            ViewBag.TimeAllocationHelperId = new SelectList(db.TimeOfCourse, "Id", "Id");
            return View();
        }

        // POST: CourseWithTimeAllocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProgramId,CourseId,CourseName,Topic,TimeAllocationHelperId,AmountOfTeachingHours")] CourseWithTimeAllocation courseWithTimeAllocation)
        {
            if (ModelState.IsValid)
            {
                db.CourseWithTimeAllocations.Add(courseWithTimeAllocation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TimeAllocationHelperId = new SelectList(db.TimeOfCourse, "Id", "Id", courseWithTimeAllocation.TimeAllocationHelperId);
            return View(courseWithTimeAllocation);
        }

        // GET: CourseWithTimeAllocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseWithTimeAllocation courseWithTimeAllocation = db.CourseWithTimeAllocations.Find(id);
            if (courseWithTimeAllocation == null)
            {
                return HttpNotFound();
            }
            ViewBag.TimeAllocationHelperId = new SelectList(db.TimeOfCourse, "Id", "Id", courseWithTimeAllocation.TimeAllocationHelperId);
            return View(courseWithTimeAllocation);
        }

        // POST: CourseWithTimeAllocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProgramId,CourseId,CourseName,Topic,TimeAllocationHelperId,AmountOfTeachingHours")] CourseWithTimeAllocation courseWithTimeAllocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseWithTimeAllocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TimeAllocationHelperId = new SelectList(db.TimeOfCourse, "Id", "Id", courseWithTimeAllocation.TimeAllocationHelperId);
            return View(courseWithTimeAllocation);
        }

        // GET: CourseWithTimeAllocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseWithTimeAllocation courseWithTimeAllocation = db.CourseWithTimeAllocations.Find(id);
            if (courseWithTimeAllocation == null)
            {
                return HttpNotFound();
            }
            return View(courseWithTimeAllocation);
        }

        // POST: CourseWithTimeAllocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseWithTimeAllocation courseWithTimeAllocation = db.CourseWithTimeAllocations.Find(id);
            db.CourseWithTimeAllocations.Remove(courseWithTimeAllocation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
