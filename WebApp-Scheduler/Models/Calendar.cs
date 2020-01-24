using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_Scheduler.Models
{
    public class Calendar
    {
        // program calendar
        public int Id { get; set; }
        public char Day { get; set; }
        public int ProgramId { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsChanged { get; set; }
        public DateTime Date { get; set;}
        public virtual ICollection<CalendarCoursePerDay> AllCoursesDays { get; set; }
    
    }

    public class CalendarCoursePerDay
    {
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public virtual Calendar CalendarDay { get; set; }
        public int CourseId { get; set; }
        public string startTimeForDay { get; set; }
        public string endTimeForDay { get; set; }
        public int TeachingHours { get; set; }
        public string Topic { get; set; }
    }
}