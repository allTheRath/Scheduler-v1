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
        public ActionResult Index(int? programId, string href="")
        {
            if (programId == null)
            {
                throw new Exception("No calenders for the program are generated.");
            }
            var program = db.Programs.Find(programId);
            ViewBag.ProgramName = program.ProgramName;
            //var calendarList = db.Calendars.Where(x => x.ProgramId == programId).ToList();
            var firstDate = program.ProgramStartDate.Value;
            var lastDate = program.ProgramEndDate.Value;
            int totalMonths = program.CalculateTotalMonthsOfStudy(program);
            List<DateTime> dates = new List<DateTime>();
            //calendarList.OrderBy(x => x.Date).ToList();
            DateHelper dateHelper = new DateHelper() { startdate = firstDate, enddate = lastDate, ProgramId = program.Id };
            ViewBag.TotalMonths = totalMonths;
            if(href == "")
            {
                return View(dateHelper);

            } else
            {
                return View(dateHelper);

            }
        }


        public ActionResult Month(DateTime? startdate, DateTime? enddate, int? monthNum, int? programId)
        {

            DateTime month = startdate.Value;
            if (monthNum != null && monthNum != 0)
            {
                month = month.AddMonths((int)monthNum);
                ViewBag.StartingDate = 0;
                ViewBag.EndingDate = 100;
            }
            else if (month != null && monthNum == 0)
            {
                ViewBag.StartingDate = (month.Day - 1);
            }
            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>() { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            DateTime st = new DateTime(month.Year, month.Month, 1);
            int startingNumber = 0;
            for (int d = 0; d < dayOfWeeks.Count(); d++)
            {
                if (dayOfWeeks[d] == st.DayOfWeek)
                {
                    startingNumber = d;
                }
            }

            ViewBag.StartingDay = startingNumber;

            if (month.Month == enddate.Value.Month && month.Year == enddate.Value.Year)
            {
                //last month 
                ViewBag.EndingDate = month.Day + 1;
            }

            if (month.Month == 2)
            {
                if (month.Year % 4 == 0 && month.Year % 100 != 0)
                {
                    ViewBag.TotalDays = 29;
                }
                else
                {
                    ViewBag.TotalDays = 28;
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
            }
            else if (month.Month >= 8 && month.Month % 2 == 0)
            {
                ViewBag.TotalDays = 31;

            }
            else if (month.Month >= 8 && month.Month % 2 == 1)
            {
                ViewBag.TotalDays = 30;
            }
            else
            {
                ViewBag.TotalDays = 0;
            }

            ViewBag.Month = month.ToString("MMMM");
            ViewBag.Year = month.Year.ToString();
            ViewBag.Date = startdate.Value;
            ViewBag.DataMonth = month.Month;
            ViewBag.DataYear = month.Year;

            SelectHolidayHelper selectHolidayHelper = new SelectHolidayHelper();
            selectHolidayHelper.CalendarForMonth = db.Calendars.Where(x => x.ProgramId == programId && x.Date.Month == month.Month).ToList();
            selectHolidayHelper.startDate = startdate.Value;
            selectHolidayHelper.endDate = enddate.Value;
            return View(selectHolidayHelper);
        }

        public ActionResult SelectHoliday(int? calendarId, int? dayNum, string previousUri)
        {
            //DateTime? startdate, DateTime? enddate, int? monthNum, int? programId;
            var cal = db.Calendars.Find(calendarId);
           
            if (cal != null && cal.Date.Day == dayNum)
            {
                if (cal.IsHoliday == true)
                {
                    cal.IsHoliday = false;
                }
                else
                {
                    cal.IsHoliday = true;
                }
                db.SaveChanges();

            }
            return Redirect(previousUri.ToString());
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
