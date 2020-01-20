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

        public ActionResult ProgramDetails(string flag = "0")
        {
            if (flag == "1")
            {
                ViewBag.Changed = false;

            }
            else
            {
                ViewBag.Changed = true;

            }
            var programOf = db.Programs.ToList().FirstOrDefault();
            if (programOf == null)
            {
                ProgramDetails program = new ProgramDetails();
                program.ProgramName = "";
                program.ProgramStartDate = DateTime.Now;
                program.ProgramEndDate = DateTime.Now;
                db.Programs.Add(program);

                db.SaveChanges();
                programOf = db.Programs.FirstOrDefault();
            }

            return View(programOf);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProgramDetails([Bind(Include = "Id,ProgramName,ProgramStartDate,ProgramEndDate,TotalTeachingHoursOfDay")] ProgramDetails program)
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

                programAlready.ProgramName = program.ProgramName;
                programAlready.TotalTeachingHoursOfDay = program.TotalTeachingHoursOfDay;

                db.SaveChanges();
                ViewBag.Changed = true;
                return RedirectToAction("Index");
            }
            return View(program);
        }
        public ActionResult InitialDatabaseEntries()
        {
            //var teachingDaysOption = db.TeachingDays.ToList();
            //if (teachingDaysOption != null && teachingDaysOption.Count() != 0)
            //{
            //    return RedirectToAction("Index");
            //}

            //char[] days = new char[] { 'M', 'T', 'W', 'R', 'F' };
            //HashSet<string> options = new HashSet<string>();

            //for (int i = 0; i < days.Length; i++)
            //{
            //    int counter = 1;


            //    while (counter <= days.Length)
            //    {
            //        string op = "";
            //        for (int k = i; k < counter; k++)
            //        {
            //            op += days[k];
            //        }
            //        if (op != "")
            //        {
            //            options.Add(op);
            //        }
            //        counter++;

            //    }

            //}


            //for (int i = 0; i < days.Length; i++)
            //{
            //    for (int j = 0; j < days.Length; j++)
            //    {
            //        if (i != j)
            //        {
            //            string opt = "" + days[i] + days[j];
            //            options.Add(opt);
            //        }
            //    }
            //}
            //foreach (var op in options)
            //{
            //    db.TeachingDays.Add(new ScheduleType() { DayOption = op });
            //}

            ProgramDetails program = new ProgramDetails();
            program.ProgramName = "";
            program.ProgramStartDate = DateTime.Now;
            program.ProgramEndDate = DateTime.Now;
            db.Programs.Add(program);

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
            int counter = 1;
            int temp = course.HoursPerDay;
            while(temp < course.ContactHours)
            {
                temp += course.HoursPerDay;
                counter++;
            }
            course.NumberOfDays = counter;
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
        

        public ActionResult CalculateSchedule()
        {
            var program = db.Programs.FirstOrDefault();

            List<TimeAllocationHelper> daysOfStudy = program.GetAllDayInstances(program);    


            var courses = db.Courses.Include(c => c.ScheduleType).ToList();
            var courseIds = courses.Select(x => x.Id).ToList();
            var prerequsiteCourses = db.PrerequisiteCourses.ToList();
            // find all courses which do not have prerequsites.
            var allCoursesWithOutPrerequsites = new List<int>();
            var allCoursesWithPrerequsites = new List<int>();
            List<SceduleHelperHolder> data = new List<SceduleHelperHolder>();
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
                for (int i = 0; i < c.ScheduleType.DayOption.Length; i++)
                {
                    holder.TeachingDays.Add(c.ScheduleType.DayOption[i]);
                }

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
                    var day = daysOfStudy[loopingIncrementor];
                    if(day.CouresIds == null)
                    {
                        day.CouresIds = new List<int>();
                    }
                    if(day.RemainingTime > 0 && c.TeachingDays.Contains(day.Day))
                    {
                        if(flag1 == true)
                        {
                            startTemp = day.Date;
                            flag1 = false;
                        }

                        if(day.RemainingTime - courseInput.HoursPerDay > 0)
                        {
                            day.CouresIds.Add(courseIDToAssignTime);
                            day.RemainingTime -= courseInput.HoursPerDay;
                            c.OverallTotalHours -= courseInput.HoursPerDay;
                            c.NoOfTeachingDays++;
                        } else
                        {
                            day.CouresIds.Add(courseIDToAssignTime);
                            c.OverallTotalHours -= day.RemainingTime;
                            day.RemainingTime = 0;
                            c.NoOfTeachingDays++;
                        }

                    }
                    loopingIncrementor++;

                }
                c.Allocated = true;
                endTemp = daysOfStudy[loopingIncrementor].Date;
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
                for(int o = 0; o < allCoursesWithPrerequsites.Count(); o++)
                {
                    var tempCourseData = data.Where(x => x.CourseId == allCoursesWithPrerequsites[o]).FirstOrDefault();       
                    if(tempCourseData.Allocated == false && tempCourseData.PrerequsiteCourseIds != null)
                    {
                        bool op = false;
                        for(int p = 0; p < tempCourseData.PrerequsiteCourseIds.Count(); p++)
                        {
                            var alReadyAssignedCourse = data.Where(x => x.CourseId == tempCourseData.PrerequsiteCourseIds[p]).FirstOrDefault();
                            if(alReadyAssignedCourse.Allocated == false)
                            {
                                op = false;
                                break;
                            } else
                            {
                                op = true;
                            }
                        }

                        if(op == true)
                        {
                            DateTime? miniMumStartDate = null;

                            for(int e = 0; e < tempCourseData.PrerequsiteCourseIds.Count(); e++)
                            {
                                if(miniMumStartDate == null)
                                {
                                    miniMumStartDate = data.Where(x => x.CourseId == tempCourseData.PrerequsiteCourseIds[e]).FirstOrDefault().EndDate;

                                } else
                                {
                                    DateTime? tempDate = data.Where(x => x.CourseId == tempCourseData.PrerequsiteCourseIds[e]).FirstOrDefault().EndDate;
                                    if(miniMumStartDate.Value.Ticks < tempDate.Value.Ticks)
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
                            while(appropriateStartingDay == true)
                            {
                                var day = daysOfStudy[loopingIncrementor];
                                if(day.Date.Ticks < miniMumStartDate.Value.Ticks)
                                {
                                    loopingIncrementor++;
                                } else
                                {
                                    appropriateStartingDay = false;
                                }
                            }

                            while (c.OverallTotalHours > 0)
                            {
                                if(loopingIncrementor >= 0 && loopingIncrementor < daysOfStudy.Count())
                                {
                                    var day = daysOfStudy[loopingIncrementor];

                                    if (day.CouresIds == null)
                                    {
                                        day.CouresIds = new List<int>();
                                    }
                                    if (day.RemainingTime > 0 && c.TeachingDays.Contains(day.Day))
                                    {
                                        if (flag1 == true)
                                        {
                                            startTemp = day.Date;
                                            flag1 = false;
                                        }
                                        if (day.RemainingTime - courseInput.HoursPerDay > 0)
                                        {
                                            day.CouresIds.Add(courseIDToAssignTime);
                                            day.RemainingTime -= courseInput.HoursPerDay;
                                            c.OverallTotalHours -= courseInput.HoursPerDay;
                                            c.NoOfTeachingDays++;
                                        }
                                        else
                                        {
                                            day.CouresIds.Add(courseIDToAssignTime);

                                            c.OverallTotalHours -= day.RemainingTime;
                                            day.RemainingTime = 0;
                                            c.NoOfTeachingDays++;
                                        }

                                    }
                                } else
                                {
                                    loopingIncrementor = 0;
                                }
                                
                                loopingIncrementor++;

                            }

                            c.Allocated = true;
                            endTemp = daysOfStudy[loopingIncrementor].Date;
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

                if(allCoursesWithPrerequsites.Count() != 0 && raceConditionCheck.Count() == allCoursesWithPrerequsites.Count())
                {
                    throw new Exception(" Some course requires prerequsite courses but all prerequsite courses are dependent on each other. Please check the input prerequsite again. ");
                    
                } else
                {
                    raceConditionCheck = new List<bool>();
                }

            }


            foreach(var courseOfInput in courses)
            {
               var tempCourseData = data.Where(x => x.CourseId == courseOfInput.Id).FirstOrDefault();
                courseOfInput.StartDate = tempCourseData.StartDate;
                courseOfInput.EndDate = tempCourseData.EndDate;
                courseOfInput.NumberOfDays = tempCourseData.NoOfTeachingDays;
            }
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
