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
            var firstDate = program.ProgramStartDate.Value;
            var lastDate = program.ProgramEndDate.Value;
            ViewBag.FirstDate = firstDate;
            ViewBag.LastDate = lastDate;
            calendarList.OrderBy(x => x.Date).ToList();

            return View(calendarList);
        }

        [HttpPost]
        public ActionResult EditMultiples()
        {

            return View();
        }

        public ActionResult Month(int? calenderId)
        {
            DateTime month = db.Calendars.Find(calenderId).Date;
            if (month.Month == 2)
            {
                // feb
                if(month.Year % 4 == 0)
                {
                    ViewBag.TotalDays = 28;
                } else
                {
                    ViewBag.TotalDays = 29;
                }
            }
            else if (month.Month < 8 && month.Month % 2 == 0)
            {
                ViewBag.TotalDays = 30;
                // months feb
            }
            else if (month.Month < 8 && month.Month % 2 == 1)
            {
                ViewBag.TotalDays = 31;
            } else if(month.Month >= 8 && month.Month % 2 == 0)
            {
                ViewBag.TotalDays = 31;

            } else if(month.Month >= 8 && month.Month % 2 == 1)
            {
                ViewBag.TotalDays = 30;
            }
            ViewBag.StartingDate = (month.Day - 1);
            ViewBag.Month = month.ToString("MMMM");
            ViewBag.Year = month.ToString("YY");
            return View();
        }

        public ActionResult DisplayCal()
        {
            return View();
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
            var calendarsList = db.Calendars.Where(x => x.ProgramId == program.Id).ToList();
            int lengthOfProgramInDays = program.CalculateTotalDaysOfEducation(program);
            DateTime startDate = program.ProgramStartDate.Value;
            DateTime endDate = program.ProgramEndDate.Value;
            if (calendarsList != null && calendarsList.Count() != 0)
            {
                var removing = calendarsList.Where(x => (x.Date < startDate) || (x.IsChanged == false)).ToList();
                foreach (var r in removing)
                {
                    db.Calendars.Remove(r);
                    calendarsList.Remove(r);

                }
                //removing all dates before program dates if any changes made in program start and end date 
                db.SaveChanges();

            }
            // creating a list of dates that are changed.
            List<DateTime> previouslyAddedChanges = calendarsList.Where(x => x.IsChanged == true).Select(x => x.Date).ToList();

            // making sure that all new changes are updated.
            var calendarsListChange = db.Calendars.Where(x => x.ProgramId == program.Id).ToList();
            DateTime maximunAlredyArrengedDateInCalendar;
            if (calendarsListChange != null && calendarsListChange.Count() != 0)
            {
                maximunAlredyArrengedDateInCalendar = calendarsListChange.Last().Date;
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
            while (startDate <= endDate)
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
                    db.Calendars.Add(c);
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
