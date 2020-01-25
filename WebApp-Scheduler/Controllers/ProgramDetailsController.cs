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
    public class ProgramDetailsController : Controller
    {
        private ScheduleContext db = new ScheduleContext();

        // GET: ProgramDetails
        public ActionResult Index()
        {
            return View(db.Programs.ToList());
        }

        // GET: ProgramDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramDetails programDetails = db.Programs.Find(id);
            if (programDetails == null)
            {
                return HttpNotFound();
            }
            return View(programDetails);
        }

        // GET: ProgramDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProgramDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProgramName,ProgramStartDate,ProgramEndDate,TotalTeachingHoursOfDay")] ProgramDetails programDetails)
        {
            if (ModelState.IsValid)
            {
                db.Programs.Add(programDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programDetails);
        }

        // GET: ProgramDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramDetails programDetails = db.Programs.Find(id);
            if (programDetails == null)
            {
                return HttpNotFound();
            }
            return View(programDetails);
        }

        // POST: ProgramDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProgramName,ProgramStartDate,ProgramEndDate,TotalTeachingHoursOfDay")] ProgramDetails program)
        {
            if (program != null)
            {
                ProgramDetails programAlready = db.Programs.Find(program.Id);
                if (programAlready != null)
                {
                    if (program.ProgramEndDate != null)
                    {
                        programAlready.ProgramEndDate = program.ProgramEndDate;

                    }
                    if (program.ProgramStartDate != null)
                    {
                        programAlready.ProgramStartDate = program.ProgramStartDate;
                    }
                    if (program.StartTime != null)
                    {
                        programAlready.StartTime = program.StartTime;
                    }
                    if (program.EndTime != null)
                    {
                        programAlready.EndTime = program.EndTime;
                    }

                    programAlready.ProgramName = program.ProgramName;
                    programAlready.TotalTeachingHoursOfDay = program.TotalTeachingHoursOfDay;

                }
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(program);
        }
        // GET: ProgramDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramDetails programDetails = db.Programs.Find(id);
            if (programDetails == null)
            {
                return HttpNotFound();
            }
            return View(programDetails);
        }

        // POST: ProgramDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProgramDetails programDetails = db.Programs.Find(id);
            db.Programs.Remove(programDetails);
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
