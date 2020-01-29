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
        public ActionResult Index(int? IdOfProgram, int ProgramId = 0)
        {
            ViewBag.ProgramId = IdOfProgram.Value;
            if (ProgramId != 0)
            {
                var courses = db.Courses.Include(c => c.ScheduleType).Where(x => x.ProgramId == ProgramId);

                return View(courses.ToList());

            }
            else if (IdOfProgram != null)
            {
                var courses = db.Courses.Include(c => c.ScheduleType).Where(x => x.ProgramId == IdOfProgram);

                return View(courses.ToList());

            }

            return View(new List<Course>());
        }

        public ActionResult UpdateCalendar(int? programId, string url = "")
        {
            if (programId == null && url == "")
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
            dateHelper.url = "";
            if (url != "")
            {
                ViewBag.Url = url;
                dateHelper.url = url;
            }
            return View(dateHelper);

        }

        public ActionResult MonthView(DateTime? startdate, DateTime? enddate, int? monthNum, int? programId)
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
            List<TimeAllocationHelper> allAssignedTimeTable = db.TimeOfCourse.Where(x => x.ProgramId == programId && x.Date.Month == month.Month && x.Date.Year == month.Year).ToList();
            List<DateTime> haveDates = allAssignedTimeTable.Select(x => x.Date).ToList();
            DateTime tempStartDate = new DateTime(month.Year, month.Month, 1);
            while (tempStartDate.Month == month.Month)
            {
                if (haveDates.Contains(tempStartDate) == false)
                {
                    TimeAllocationHelper tempAllocator = new TimeAllocationHelper() { Id = 0, Date = tempStartDate, Day = 'N', ProgramId = programId.Value };
                    allAssignedTimeTable.Add(tempAllocator);
                }

                tempStartDate = tempStartDate.AddDays(1);
            }
            var TempProgram = db.Programs.Find(programId);
            List<HelperOfDateHoliday> allHolidaysForThisMonthDates = db.Calendars.Where(x => x.ProgramId == programId && x.Date.Month == month.Month && x.Date.Year == month.Year).ToList().Select(x => new HelperOfDateHoliday() { Date = x.Date, IsHoliday = x.IsHoliday }).ToList();
            EditCalendarHelper editCalendarHelper = new EditCalendarHelper() { DateHelper = allAssignedTimeTable.OrderBy(x => x.Date).ToList(), startDate = startdate.Value, endDate = enddate.Value, ProgramTeachingHoursPerDay = TempProgram.TotalTeachingHoursOfDay, month = month.Month, year = month.Year, ProgramId = programId.Value };
            List<bool> flagsForHolidays = new List<bool>();
            foreach (var op in editCalendarHelper.DateHelper)
            {
                var dd = allHolidaysForThisMonthDates.Find(x => x.Date == op.Date);
                if (dd == null)
                {
                    flagsForHolidays.Add(true);
                }
                else
                {
                    if (dd.IsHoliday == true)
                    {
                        flagsForHolidays.Add(true);
                    }
                    else
                    {
                        flagsForHolidays.Add(false);
                    }
                }
            }
            editCalendarHelper.holidays = flagsForHolidays;

            return View(editCalendarHelper);

        }

        public ActionResult PartialDay(int? timehelperId, int? ProgramTeachingHoursPerDay, int? month, int? year, int? day, int? programid)
        {
            ViewBag.ProgramTeachingHoursPerDay = (int)ProgramTeachingHoursPerDay;
            if (timehelperId != null && timehelperId != 0)
            {
                var singleDayReference = db.TimeOfCourse.Find(timehelperId);
                var allCoursesDetails = db.CourseWithTimeAllocations.Where(x => x.TimeAllocationHelperId == singleDayReference.Id).ToList();
                ViewBag.TimeId = timehelperId.Value;

                return View(allCoursesDetails);
            }
            DateTime timeOfDate = new DateTime(year.Value, month.Value, day.Value);
            TimeAllocationHelper timeAllocationHelper = new TimeAllocationHelper();
            timeAllocationHelper.Date = timeOfDate;
            timeAllocationHelper.ProgramId = programid.Value;

            timeAllocationHelper.RemainingTime = db.Programs.Find(programid).TotalTeachingHoursOfDay;
            db.TimeOfCourse.Add(timeAllocationHelper);
            db.SaveChanges();
            var ins = db.TimeOfCourse.Where(x => x.Date == timeOfDate && x.ProgramId == programid).ToList().FirstOrDefault();
            ViewBag.TimeId = ins.Id;
            return View(new List<CourseWithTimeAllocation>());
        }


        public ActionResult CoursesDetailsForProgram(int? ProgramId)
        {
            var Courses = db.ScheduleHelperContextPerCourse.Where(X => X.ProgramId == ProgramId).ToList();

            return View(Courses);
            //return View();
        }
        public ActionResult AddHoursInDay(int? TimeAllocationHelperId, string url = "")
        {
            var timeAllocationInstance = db.TimeOfCourse.Find(TimeAllocationHelperId);

            var courses = db.Courses.Where(x => x.ProgramId == timeAllocationInstance.ProgramId).ToList();

            ViewBag.Id = new SelectList(courses, "Id", "CourseName");
            AddCourseInDayViewModel addCourseInDayViewModel = new AddCourseInDayViewModel() { RemainingTime = timeAllocationInstance.RemainingTime, TimeAllocationId = timeAllocationInstance.Id, url = url, AmountOfTeachingHours = 0, Topic = "" };
            return View(addCourseInDayViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHoursInDay([Bind(Include = "Id,Topic,AmountOfTeachingHours,TimeAllocationId,RemainingTime,url")] AddCourseInDayViewModel addCourseInDayViewModel)
        {
            var timeAllocationInstance = db.TimeOfCourse.Find(addCourseInDayViewModel.TimeAllocationId);

            var course = db.Courses.Find(addCourseInDayViewModel.Id);
            if (course != null)
            {
                CourseWithTimeAllocation courseWithTimeAllocation = new CourseWithTimeAllocation() { CourseId = addCourseInDayViewModel.Id, CourseName = course.CourseName, AmountOfTeachingHours = addCourseInDayViewModel.AmountOfTeachingHours, ProgramId = course.ProgramId, TimeAllocationHelperId = addCourseInDayViewModel.TimeAllocationId, Topic = addCourseInDayViewModel.Topic };
                timeAllocationInstance.RemainingTime -= addCourseInDayViewModel.RemainingTime;
                db.CourseWithTimeAllocations.Add(courseWithTimeAllocation);
                db.SaveChanges();
                var instanceOfCourseDataHelper = db.ScheduleHelperContextPerCourse.Where(x => x.courseId == course.Id).FirstOrDefault();

                instanceOfCourseDataHelper.OverallTotalHours -= addCourseInDayViewModel.AmountOfTeachingHours;

                instanceOfCourseDataHelper.NoOfTeachingDays += 1;
                if (course.EndDate < timeAllocationInstance.Date)
                {
                    course.EndDate = timeAllocationInstance.Date;
                }
                db.SaveChanges();

                string path = addCourseInDayViewModel.url;
                path += "#" + timeAllocationInstance.Date.ToString("MMMM") + "-" + timeAllocationInstance.Date.Year;
                return Redirect(path);
            }
            ViewBag.Id = new SelectList(db.Courses.ToList(), "Id", "CourseName");

            return View();
        }

        public ActionResult AddDetailInCalendar(int? calendarId, int? dayNum, string previousUri)
        {
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
            return RedirectToAction("UpdateCalendar", new { programId = cal.ProgramId, url = previousUri.ToString() });

        }

        public ActionResult EditDay(int? id, string url = "")
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

            CourseWithTimeAllocationsViewModel courseWithTimeAllocationsViewModel = new CourseWithTimeAllocationsViewModel() { Id = courseWithTimeAllocation.Id, CourseId = courseWithTimeAllocation.CourseId, Course = courseWithTimeAllocation.CourseName, Delete = false, TeachingHours = courseWithTimeAllocation.AmountOfTeachingHours, Topic = courseWithTimeAllocation.Topic, TimeAllocationHelperId = courseWithTimeAllocation.TimeAllocationHelperId, url = url };

            return View(courseWithTimeAllocationsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDay([Bind(Include = "Id,TimeAllocationHelperId,url,Course, CourseId,Topic,TeachingHours,Delete")] CourseWithTimeAllocationsViewModel courseWithTimeAllocation)
        {

            var temp = courseWithTimeAllocation;
            int previousHoursForDay = db.CourseWithTimeAllocations.Find(courseWithTimeAllocation.Id).AmountOfTeachingHours;
            if (courseWithTimeAllocation != null && courseWithTimeAllocation.Delete == false)
            {
                var instanceOfTimeAllocationForCourse = db.CourseWithTimeAllocations.Find(courseWithTimeAllocation.Id);
                instanceOfTimeAllocationForCourse.CourseName = courseWithTimeAllocation.Course;
                instanceOfTimeAllocationForCourse.Topic = courseWithTimeAllocation.Topic;
                instanceOfTimeAllocationForCourse.AmountOfTeachingHours = courseWithTimeAllocation.TeachingHours;
                var instanceOfCourseDataHelper = db.ScheduleHelperContextPerCourse.Where(x => x.courseId == instanceOfTimeAllocationForCourse.CourseId).ToList().FirstOrDefault();
                // negative
                instanceOfCourseDataHelper.OverallTotalHours += previousHoursForDay - courseWithTimeAllocation.TeachingHours;
                var timeInstanceForUpdationOfCourse = db.TimeOfCourse.Find(courseWithTimeAllocation.TimeAllocationHelperId);

                var courseUpdation = db.Courses.Find(courseWithTimeAllocation.CourseId);
                if (courseUpdation.StartDate > timeInstanceForUpdationOfCourse.Date)
                {
                    courseUpdation.StartDate = timeInstanceForUpdationOfCourse.Date;
                }
                else if (courseUpdation.EndDate < timeInstanceForUpdationOfCourse.Date)
                {
                    courseUpdation.EndDate = timeInstanceForUpdationOfCourse.Date;
                }

                db.SaveChanges();
            }
            if (courseWithTimeAllocation.Delete == true)
            {
                var instanceOfTimeAllocationToDelete = db.CourseWithTimeAllocations.Find(courseWithTimeAllocation.Id);
                var instanceOfCourseDataHelper = db.ScheduleHelperContextPerCourse.ToList().Where(x => x.courseId == instanceOfTimeAllocationToDelete.CourseId).FirstOrDefault();
                // negative
                instanceOfCourseDataHelper.OverallTotalHours += previousHoursForDay;

                db.CourseWithTimeAllocations.Remove(instanceOfTimeAllocationToDelete);
                var timeInstanceForUpdationOfCourse = db.TimeOfCourse.Find(courseWithTimeAllocation.TimeAllocationHelperId);


                var courseUpdation = db.Courses.Find(courseWithTimeAllocation.CourseId);
                if (true)
                {
                    // when start day is deleted or end day is deleted.. Otherwise it does't matter. 
                    var timeData = db.TimeOfCourse.Where(x => x.ProgramId == timeInstanceForUpdationOfCourse.ProgramId).ToList().OrderBy(x => x.Date).ToList();
                    if (timeInstanceForUpdationOfCourse.Date == courseUpdation.StartDate)
                    {
                        bool flagOf = false;
                        // start date is deleting
                        for (int i = 0; i < timeData.Count(); i++)
                        {
                            if (flagOf == true)
                            {
                                int helperId = timeData[i].Id;
                                // id is passed. now look for an instance where course id matches.
                                var firstInstance = db.CourseWithTimeAllocations.Where(x => x.CourseId == courseUpdation.Id && x.TimeAllocationHelperId == helperId).ToList().FirstOrDefault();
                                if (firstInstance != null)
                                {
                                    // timeallocation id with the matching date is new start date.
                                    courseUpdation.StartDate = timeData[i].Date;
                                    db.SaveChanges();
                                    break;
                                }
                            }
                            if (timeData[i].Date == timeInstanceForUpdationOfCourse.Date)
                            {
                                flagOf = true;
                            }
                        }

                    }
                    else if(timeInstanceForUpdationOfCourse.Date == courseUpdation.EndDate)
                    {
                        // end date is deleting

                        bool flagOf = false;
                        // start date is deleting
                        for (int i = timeData.Count() - 1; i >= 0; i--)
                        {
                            if (flagOf == true)
                            {
                                // id is passed. now look for an instance where course id matches.
                                int helperId = timeData[i].Id;

                                var firstInstance = db.CourseWithTimeAllocations.Where(x => x.CourseId == courseUpdation.Id && x.TimeAllocationHelperId == helperId).ToList().FirstOrDefault();

    
                                if (firstInstance != null)
                                {
                                    // timeallocation id with the matching date is new start date.
                                    courseUpdation.EndDate = timeData[i].Date;
                                    db.SaveChanges();
                                    break;
                                }
                            }
                            if (timeData[i].Date == timeInstanceForUpdationOfCourse.Date)
                            {
                                flagOf = true;
                            }
                        }
                    }


                }

                db.SaveChanges();
            }
            if (temp != null)
            {

                var timeInstanceForRedirectionByDate = db.TimeOfCourse.Find(courseWithTimeAllocation.TimeAllocationHelperId);
                DateTime monthToGoTo = timeInstanceForRedirectionByDate.Date;

                string path = temp.url + "#" + monthToGoTo.ToString("MMMM") + "-" + monthToGoTo.Year.ToString();
                return Redirect(path);

            }


            return View(courseWithTimeAllocation);
        }

        public ActionResult ProgramDetails(int? Id, string flag = "0", string flag2 = "3")
        {

            if (flag == "1")
            {
                ViewBag.Changed = false;

            }
            else
            {
                ViewBag.Changed = true;

            }

            var programOf = db.Programs.Find(Id);
            if (flag2 == "2")
            {
                ViewBag.DisplayButton = false;
            }
            return View(programOf);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProgramDetails([Bind(Include = "Id,ProgramName,ProgramStartDate,ProgramEndDate,TotalTeachingHoursOfDay,StartTime,EndTime")] ProgramDetails program)
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

                db.SaveChanges();
                ViewBag.Changed = true;
                return RedirectToAction("Index", new { IdOfProgram = programAlready.Id });
            }
            return View(program);
        }
        public ActionResult InitialDatabaseEntries()
        {
            var teachingDaysOption = db.TeachingDays.ToList();
            //if (teachingDaysOption != null && teachingDaysOption.Count() != 0)
            //{
            //    return RedirectToAction("Index", "Programs");
            //}

            char[] days = new char[] { 'M', 'T', 'W', 'R', 'F' };
            HashSet<string> options = new HashSet<string>();

            for (int i = 0; i < days.Length; i++)
            {
                int counter = 1;
                string op = "";
                op += "" + days[i];
                while (counter <= days.Length)
                {

                    for (int k = i + 1; k < counter; k++)
                    {
                        op += days[k];
                        if (op != "")
                        {
                            options.Add(op);
                        }

                    }
                    op = "" + days[i];

                    counter++;

                }

            }


            for (int i = 0; i < days.Length; i++)
            {
                for (int j = days.Length - 1; j > 0; j--)
                {
                    if (i != j)
                    {
                        string opt = "" + days[i] + days[j];
                        options.Add(opt);
                    }
                }
            }

            options.Add("MWF");
            options.Add("TWF");

            options.Add("WRF");
            options.Add("TWR");
            options.Add("TRF");
            options.Add("MRF");
            options.Add("MTW");
            options.Add("MWF");
            options.Add("MRF");
            options.Add("MTR");
            options.Add("MTF");
            options.Add("TWF");
            options.Add("WRF");

            foreach (var op in options)
            {
                if (teachingDaysOption.Where(x => x.DayOption == op).FirstOrDefault() == null)
                {
                    db.TeachingDays.Add(new ScheduleType() { DayOption = op });
                    db.SaveChanges();
                }
                else
                {
                    //just in case multiple entries present.
                    var extra = teachingDaysOption.Where(x => x.DayOption == op).ToList();
                    for (var j = 1; j < extra.Count(); j++)
                    {
                        db.TeachingDays.Remove(extra[j]);
                    }
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "ProgramDetails");
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
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ProgramName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CourseCode,CourseName,Instructor,ContactHours,HoursPerDay,NumberOfDays,StartDate,EndDate,ScheduleTypeId,ProgramId")] Course course)
        {
            int counter = 1;
            int temp = course.HoursPerDay;
            while (temp < course.ContactHours)
            {
                temp += course.HoursPerDay;
                counter++;
            }
            course.NumberOfDays = counter;
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index", new { IdOfProgram = course.ProgramId });
            }

            ViewBag.ScheduleTypeId = new SelectList(db.TeachingDays, "Id", "DayOption", course.ScheduleTypeId);
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ProgramName", course.ProgramId);
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
            var selectDayOptions = db.TeachingDays.OrderBy(X => X.DayOption.Length).ToList();
            ViewBag.ScheduleTypeId = new SelectList(selectDayOptions, "Id", "DayOption", course.ScheduleTypeId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CourseCode,CourseName,Instructor,ContactHours,HoursPerDay,NumberOfDays,StartDate,EndDate,ScheduleTypeId,ProgramId")] Course course)
        {
            int counter = 1;
            int temp = course.HoursPerDay;
            while (temp < course.ContactHours)
            {
                temp += course.HoursPerDay;
                counter++;
            }
            course.NumberOfDays = counter;
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { IdOfProgram = course.ProgramId });
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
            var prerequsites = db.PrerequisiteCourses.Where(x => x.ActualCourseId == id || x.RequiredCourseId == id).ToList();
            foreach (var r in prerequsites)
            {
                db.PrerequisiteCourses.Remove(r);
            }

            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index", new { IdOfProgram = course.ProgramId });
        }

        public ActionResult AddPrerequisite(int? courseId)
        {
            if (courseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var thisCourse = db.Courses.Find(courseId);
            var courses = db.Courses.ToList().Where(x => x.Id != courseId).ToList();
            var alredySelectedCourseIds = db.PrerequisiteCourses.ToList().Where(x => x.ActualCourseId == courseId).ToList().Select(x => x.RequiredCourseId).ToList();
            var selectionOptions = courses.Where(x => alredySelectedCourseIds.Contains(x.Id) == false && x.ProgramId == thisCourse.ProgramId).ToList().Select(x => new CourseSelectionViewModel() { CourseName = x.CourseName, Id = x.Id }).ToList();
            var alreadySelected = courses.Where(x => alredySelectedCourseIds.Contains(x.Id)).ToList().Select(x => new CourseSelectionViewModel() { CourseName = x.CourseName, Id = x.Id }).ToList();

            ListOfCourseSelectionViewModel listOfCourseSelectionViewModel = new ListOfCourseSelectionViewModel();
            listOfCourseSelectionViewModel.CourseId = Convert.ToInt32(courseId);
            listOfCourseSelectionViewModel.selectedCourses = alreadySelected;
            listOfCourseSelectionViewModel.selectOptions = selectionOptions;
            listOfCourseSelectionViewModel.IdOfProgram = thisCourse.ProgramId;
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


        public ActionResult CalculateSchedule(int? programId)
        {

            if (programId == null)
            {
                throw new Exception("Program id was not present.");
            }
            var program = db.Programs.Find(programId);

            List<TimeAllocationHelper> daysOfStudy = program.GetAllDayInstances(program, db);


            var courses = db.Courses.Include(c => c.ScheduleType).Where(x => x.ProgramId == programId).ToList();
            foreach (var c in courses)
            {
                if (c.HoursPerDay > program.TotalTeachingHoursOfDay)
                {
                    throw new Exception("Course " + c.CourseName + " has " + c.HoursPerDay + "\n But program has " + program.ProgramName + " has " + program.TotalTeachingHoursOfDay + " .\n please update the inputs and try again.");
                }
            }
            var courseIds = courses.Select(x => x.Id).ToList();
            var prerequsiteCourses = db.PrerequisiteCourses.Where(x => courseIds.Contains(x.ActualCourseId)).ToList();
            // find all courses which do not have prerequsites.
            var allCoursesWithOutPrerequsites = new List<int>();
            var allCoursesWithPrerequsites = new List<int>();
            List<SceduleHelperHolder> data = new List<SceduleHelperHolder>();
            List<ScheduleHelperContextData> saveDataForCourse = new List<ScheduleHelperContextData>();
            foreach (var c in courses)
            {
                var checking = prerequsiteCourses.Where(x => x.ActualCourseId == c.Id).FirstOrDefault();
                if (checking == null)
                {
                    allCoursesWithOutPrerequsites.Add(c.Id);
                }
                else
                {
                    allCoursesWithPrerequsites.Add(c.Id);
                }
                SceduleHelperHolder holder = new SceduleHelperHolder();
                holder.PrerequsiteCourseIds = prerequsiteCourses.Where(x => x.ActualCourseId == c.Id).ToList().Select(x => x.RequiredCourseId).ToList();
                holder.Allocated = false;
                holder.CourseId = c.Id;

                holder.CourseName = c.CourseName;
                holder.TotalHoursPerDay = c.HoursPerDay;
                holder.OverallTotalHours = c.ContactHours;
                holder.TeachingDays = new List<char>();
                holder.NoOfTeachingDays = 0;
                string daysString = "";
                for (int i = 0; i < c.ScheduleType.DayOption.Length; i++)
                {
                    daysString += c.ScheduleType.DayOption[i];
                    holder.TeachingDays.Add(c.ScheduleType.DayOption[i]);
                }
                saveDataForCourse.Add(new ScheduleHelperContextData() { courseId = c.Id, CourseName = c.CourseName, NoOfTeachingDays = (int)c.NumberOfDays, OverallTotalHours = c.ContactHours, ProgramId = c.ProgramId, TeachingDays = daysString, TotalHoursPerDay = c.HoursPerDay });
                data.Add(holder);
            }




            // allocating course with it's aloowed time per day..
            int loopingCounter = 0;
            while (loopingCounter < allCoursesWithOutPrerequsites.Count())
            {
                // tempcounter increments at assigning time..
                var courseIDToAssignTime = allCoursesWithOutPrerequsites[loopingCounter];
                var courseInput = courses.Where(x => x.Id == courseIDToAssignTime).FirstOrDefault();
                int loopingIncrementor = 0;

                var c = data.Where(x => x.CourseId == courseIDToAssignTime).FirstOrDefault();
                DateTime? startTemp = null;
                DateTime? endTemp = null;
                bool flag1 = true;

                while (c.OverallTotalHours > 0)
                {
                    if (loopingIncrementor == daysOfStudy.Count() || loopingIncrementor < 0)
                    {
                        loopingIncrementor = 0;
                    }
                    var day = daysOfStudy[loopingIncrementor];

                    if (day.CouresIds == null)
                    {
                        day.CouresIds = new List<int>();
                    }
                    if (day.AllocatedTimes == null)
                    {
                        day.AllocatedTimes = new List<int>();
                    }
                    if (day.RemainingTime > 0 && c.TeachingDays.Contains(day.Day))
                    {
                        if (flag1 == true && day.RemainingTime - courseInput.HoursPerDay >= 0)
                        {
                            startTemp = day.Date;
                            flag1 = false;
                        }

                        if (day.RemainingTime - courseInput.HoursPerDay >= 0)
                        {

                            day.CouresIds.Add(courseIDToAssignTime);
                            day.AllocatedTimes.Add(courseInput.HoursPerDay);
                            day.RemainingTime -= courseInput.HoursPerDay;
                            c.OverallTotalHours -= courseInput.HoursPerDay;
                            c.NoOfTeachingDays++;
                            endTemp = day.Date;
                        }
                        //else
                        //{
                        //    day.CouresIds.Add(courseIDToAssignTime);
                        //    day.AllocatedTimes.Add(day.RemainingTime);
                        //    c.OverallTotalHours -= day.RemainingTime;
                        //    day.RemainingTime = 0;
                        //    c.NoOfTeachingDays++;
                        //}

                    }
                    loopingIncrementor++;

                }
                c.Allocated = true;

                //ading calculated no of hours in database
                var instanceOfSaveCourseInfo = saveDataForCourse.Where(x => x.courseId == c.CourseId).ToList().FirstOrDefault();
                instanceOfSaveCourseInfo.OverallTotalHours = c.OverallTotalHours;

                c.StartDate = startTemp;
                c.EndDate = endTemp;
                loopingCounter++;
            }

            List<bool> endConditionCheck = new List<bool>();
            List<bool> raceConditionCheck = new List<bool>();
            int howManyLeft = allCoursesWithPrerequsites.Count();
            while (endConditionCheck.Count() < howManyLeft)
            {
                // running untill all courses are allocated..
                //selecting course
                for (int o = 0; o < allCoursesWithPrerequsites.Count(); o++)
                {
                    var tempCourseData = data.Where(x => x.CourseId == allCoursesWithPrerequsites[o]).FirstOrDefault();
                    if (tempCourseData.Allocated == false && tempCourseData.PrerequsiteCourseIds != null)
                    {
                        bool op = false;
                        for (int p = 0; p < tempCourseData.PrerequsiteCourseIds.Count(); p++)
                        {
                            var alReadyAssignedCourse = data.Where(x => x.CourseId == tempCourseData.PrerequsiteCourseIds[p]).FirstOrDefault();

                            if (alReadyAssignedCourse.Allocated == false)
                            {
                                op = false;
                                break;
                            }
                            else
                            {
                                op = true;
                            }
                        }

                        if (op == true)
                        {
                            DateTime? miniMumStartDate = null;

                            for (int e = 0; e < tempCourseData.PrerequsiteCourseIds.Count(); e++)
                            {
                                if (miniMumStartDate == null)
                                {
                                    miniMumStartDate = data.Where(x => x.CourseId == tempCourseData.PrerequsiteCourseIds[e]).FirstOrDefault().EndDate;

                                }
                                else
                                {
                                    DateTime? tempDate = data.Where(x => x.CourseId == tempCourseData.PrerequsiteCourseIds[e]).FirstOrDefault().EndDate;
                                    if (miniMumStartDate.Value.Ticks < tempDate.Value.Ticks)
                                    {
                                        miniMumStartDate = tempDate;
                                    }
                                }

                            }



                            var courseIDToAssignTime = tempCourseData.CourseId;
                            var courseInput = courses.Where(x => x.Id == courseIDToAssignTime).FirstOrDefault();
                            int loopingIncrementor = 0;

                            var c = tempCourseData;
                            DateTime? startTemp = null;
                            DateTime? endTemp = null;
                            bool flag1 = true;


                            bool appropriateStartingDay = true;
                            while (appropriateStartingDay == true)
                            {
                                var day = daysOfStudy[loopingIncrementor];
                                if (day.Date.Ticks < miniMumStartDate.Value.Ticks)
                                {
                                    loopingIncrementor++;
                                }
                                else
                                {
                                    appropriateStartingDay = false;
                                }
                            }

                            while (c.OverallTotalHours > 0)
                            {
                                if (loopingIncrementor >= 0 && loopingIncrementor < daysOfStudy.Count())
                                {
                                    var day = daysOfStudy[loopingIncrementor];

                                    if (day.CouresIds == null)
                                    {
                                        day.CouresIds = new List<int>();
                                    }
                                    if (day.AllocatedTimes == null)
                                    {
                                        day.AllocatedTimes = new List<int>();
                                    }
                                    if (day.RemainingTime > 0 && c.TeachingDays.Contains(day.Day))
                                    {
                                        if (flag1 == true && day.RemainingTime - courseInput.HoursPerDay >= 0)
                                        {
                                            startTemp = day.Date;
                                            flag1 = false;
                                        }
                                        if (day.RemainingTime - courseInput.HoursPerDay >= 0)
                                        {
                                            day.CouresIds.Add(courseIDToAssignTime);
                                            day.AllocatedTimes.Add(courseInput.HoursPerDay);
                                            day.RemainingTime -= courseInput.HoursPerDay;
                                            c.OverallTotalHours -= courseInput.HoursPerDay;
                                            c.NoOfTeachingDays++;
                                            endTemp = day.Date;
                                        }
                                        //else
                                        //{

                                        //    //day.CouresIds.Add(courseIDToAssignTime);
                                        //    //day.AllocatedTimes.Add(day.RemainingTime);
                                        //    //c.OverallTotalHours -= day.RemainingTime;
                                        //    //day.RemainingTime = 0;
                                        //    //c.NoOfTeachingDays++;
                                        //}

                                    }
                                }
                                else
                                {
                                    loopingIncrementor = 0;
                                }

                                loopingIncrementor++;

                            }

                            c.Allocated = true;
                            (saveDataForCourse.Where(x => x.courseId == c.CourseId).ToList().FirstOrDefault()).OverallTotalHours = c.OverallTotalHours;

                            allCoursesWithPrerequsites.Remove(c.CourseId);
                            c.StartDate = startTemp;
                            c.EndDate = endTemp;
                            endConditionCheck.Add(true);
                        }
                        else
                        {
                            raceConditionCheck.Add(false);
                        }
                    }
                }

                if (allCoursesWithPrerequsites.Count() != 0 && raceConditionCheck.Count() == allCoursesWithPrerequsites.Count())
                {
                    throw new Exception(" Some course requires prerequsite courses but all prerequsite courses are dependent on each other. Please check the input prerequsite again. ");

                }
                else
                {
                    raceConditionCheck = new List<bool>();
                }

            }


            foreach (var courseOfInput in courses)
            {
                var tempCourseData = data.Where(x => x.CourseId == courseOfInput.Id).FirstOrDefault();
                courseOfInput.StartDate = tempCourseData.StartDate;
                courseOfInput.EndDate = tempCourseData.EndDate;
                courseOfInput.NumberOfDays = tempCourseData.NoOfTeachingDays;
                db.SaveChanges();
            }


            //daysOfStudy
            var programTimeLineData = db.TimeOfCourse.Where(x => x.ProgramId == program.Id).ToList().OrderBy(x => x.Date).ToList();
            foreach (var r in programTimeLineData)
            {
                db.TimeOfCourse.Remove(r);
            }
            db.SaveChanges();

            var previouseCalculatedInstances = db.CourseWithTimeAllocations.Where(x => x.ProgramId == program.Id).ToList();
            foreach (var op in previouseCalculatedInstances)
            {
                db.CourseWithTimeAllocations.Remove(op);
                db.SaveChanges();
            }

            foreach (var dayIns in daysOfStudy)
            {
                db.TimeOfCourse.Add(dayIns);
                db.SaveChanges();
            }


            var newInstancesOfTimeOfCourse = db.TimeOfCourse.Where(x => x.ProgramId == program.Id).ToList();
            foreach (var newIns in newInstancesOfTimeOfCourse)
            {
                var op = daysOfStudy.Find(x => x.Date == newIns.Date);
                if (op != null && op.CouresIds != null)
                {
                    int counterI = 0;

                    for (int pq = 0; pq < op.CouresIds.Count(); pq++)
                    {
                        CourseWithTimeAllocation courseWithTimeAllocation = new CourseWithTimeAllocation();
                        courseWithTimeAllocation.TimeAllocationHelperId = newIns.Id;
                        courseWithTimeAllocation.CourseId = op.CouresIds[pq];
                        courseWithTimeAllocation.AmountOfTeachingHours = op.AllocatedTimes[counterI];
                        counterI++;
                        courseWithTimeAllocation.ProgramId = op.ProgramId;
                        courseWithTimeAllocation.CourseName = db.Courses.Find(op.CouresIds[pq]).CourseName;
                        courseWithTimeAllocation.Topic = "";
                        db.CourseWithTimeAllocations.Add(courseWithTimeAllocation);
                        db.SaveChanges();

                    }
                }


            }
            foreach (var c in saveDataForCourse)
            {
                var instanceOfDbEntry = db.ScheduleHelperContextPerCourse.Where(x => x.courseId == c.courseId).FirstOrDefault();
                if (instanceOfDbEntry != null)
                {
                    //already there then update.
                    instanceOfDbEntry.CourseName = c.CourseName;
                    instanceOfDbEntry.NoOfTeachingDays = c.NoOfTeachingDays;
                    instanceOfDbEntry.OverallTotalHours = c.OverallTotalHours;
                    instanceOfDbEntry.TeachingDays = c.TeachingDays;
                    instanceOfDbEntry.TotalHoursPerDay = c.TotalHoursPerDay;
                    db.SaveChanges();
                }
                else
                {
                    //else create new instance of helper data.
                    db.ScheduleHelperContextPerCourse.Add(c);
                    db.SaveChanges();
                }

            }

            return RedirectToAction("UpdateCalendar", new { programId = program.Id });
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
