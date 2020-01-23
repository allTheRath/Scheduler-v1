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
    public class CalendarsController : Controller
    {
        private ScheduleContext db = new ScheduleContext();

        // GET: Calendars
        public ActionResult Index()
        {
            return View(db.Calendars.ToList());
        }

        // GET: Calendars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calendar calendar = db.Calendars.Find(id);
            if (calendar == null)
            {
                return HttpNotFound();
            }
            return View(calendar);
        }

        // GET: Calendars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Calendars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsHoliday,Date")] Calendar calendar)
        {
            if (ModelState.IsValid)
            {
                db.Calendars.Add(calendar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(calendar);
        }

        // GET: Calendars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calendar calendar = db.Calendars.Find(id);
            if (calendar == null)
            {
                return HttpNotFound();
            }
            return View(calendar);
        }

        // POST: Calendars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsHoliday,Date")] Calendar calendar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calendar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(calendar);
        }

        // GET: Calendars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calendar calendar = db.Calendars.Find(id);
            if (calendar == null)
            {
                return HttpNotFound();
            }
            return View(calendar);
        }

        // POST: Calendars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Calendar calendar = db.Calendars.Find(id);
            db.Calendars.Remove(calendar);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ProgramTimeline()
        {
            // runs only one time after creating program.
            var program = db.Programs.FirstOrDefault();
            if (program == null || program.ProgramStartDate == null || program.ProgramEndDate == null)
            {
                throw new Exception("Program has to have a valid start end end date.");
            }
            if (db.Calendars.ToList() != null)
            {
                //remove all previous days.
                db.Calendars.ToList().RemoveAll(x => x.IsHoliday == false || x.IsHoliday == true);
                // removes all dates 
                db.SaveChanges();
            }
            char[] days = new char[] { 'U', 'M', 'T', 'W', 'R', 'F', 'S' };
            var calendarsList = db.Calendars.ToList();
            int lengthOfProgramInDays = program.CalculateTotalDaysOfEducation(program);
            DateTime startDate = program.ProgramStartDate.Value;
            for (int i = 0; i < lengthOfProgramInDays; i++)
            {
                Calendar c = new Calendar();
                c.Date = startDate;
                int a = (int)startDate.DayOfWeek;
                c.Day = days[a];
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    c.IsHoliday = false;

                }
                else
                {
                    c.IsHoliday = true;

                }

                calendarsList.Add(c);
                startDate = startDate.AddDays(1);
            }
            db.SaveChanges();

            return View();
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
