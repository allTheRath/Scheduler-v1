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
        public ActionResult Index(int? programId)
        {
            if (programId == null)
            {
                throw new Exception("No calenders for the program are generated.");
            }
            var program = db.Programs.Find(programId);
            ViewBag.ProgramName = program.ProgramName;
            var calendarList = db.Calendars.Where(x => x.ProgramId == programId).ToList();
            return View(calendarList);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,IsHoliday,Date")] Calendar calendar)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(calendar).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(calendar);
        //}


        public ActionResult ProgramTimeline(int? ProgramId)
        {
            // runs only one time after creating program.
            var program = db.Programs.Find(ProgramId);
            if (program == null || program.ProgramStartDate == null || program.ProgramEndDate == null)
            {
                throw new Exception("Program has to have a valid start end end date.");
            }

            char[] days = new char[] { 'U', 'M', 'T', 'W', 'R', 'F', 'S' };
            var calendarsList = db.Calendars.Include(x => x.AllCoursesDays).Where(x => x.ProgramId == program.Id).ToList();
            int lengthOfProgramInDays = program.CalculateTotalDaysOfEducation(program);
            DateTime startDate = program.ProgramStartDate.Value;

            if (calendarsList != null && calendarsList.Count() != 0)
            {
                calendarsList.RemoveAll(x => (x.Date < startDate) || (x.IsChanged == false));
                //removing all dates before program dates if any changes made in program start and end date 
                db.SaveChanges();

            }
            // creating a list of dates that are changed.
            List<DateTime> previouslyAddedChanges = calendarsList.Where(x => x.IsChanged == true).Select(x => x.Date).ToList();

            // making sure that all new changes are updated.
            calendarsList = db.Calendars.Where(x => x.ProgramId == program.Id).ToList();
            DateTime maximunAlredyArrengedDateInCalendar;
            if (calendarsList != null && calendarsList.Count() != 0)
            {
                maximunAlredyArrengedDateInCalendar = calendarsList.Last().Date;
            }
            else
            {
                maximunAlredyArrengedDateInCalendar = startDate.AddDays(-1);
            }

            int idOfProgram = program.Id;
            if (previouslyAddedChanges == null)
            {
                // no null pt ex for below iteration.
                previouslyAddedChanges = new List<DateTime>();
            }
            for (int i = 0; i < lengthOfProgramInDays; i++)
            {

                if (startDate > maximunAlredyArrengedDateInCalendar && (previouslyAddedChanges.Contains(startDate) == false))
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
                    c.ProgramId = idOfProgram;
                    calendarsList.Add(c);

                }
                startDate = startDate.AddDays(1);
            }
            db.SaveChanges();
            // initilizing calander days for given program or changed start end date for program



            return RedirectToAction("Index", new { programId = idOfProgram });

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
