using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebApp_Scheduler.Models
{
    public class Calendar
    {
        // program calendar
        public int Id { get; set; }
        public char Day { get; set; }
        [DisplayName("Program ")]
        public int ProgramId { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsChanged { get; set; }
        public DateTime Date { get; set;}
        
    }
    public class DateHelper
    {
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int ProgramId { get; set; }
        public string url { get; set; }
    }

    public class SelectHolidayHelper
    {
        public List<Calendar> CalendarForMonth { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }
    public class HelperForPartialDayWithTimeallocationHelper
    {
        public int Id { get; set; }
        [DisplayName("Program")]
        public int ProgramId { get; set; }
        public DateTime Date { get; set; }
        public char Day { get; set; }
        public List<int> CouresIds { get; set; }
        public int RemainingTime { get; set; }

    }

    public class HelperOfDateHoliday
    {
        public bool IsHoliday { get; set; }
        public DateTime Date { get; set; }
    }


    public class AddCourseInDayViewModel
    {//Id will be course id
        [DisplayName("Course ")]
        public int Id { get; set; }
        public string Topic { get; set; }
        [DisplayName("Teaching Hours")]
        public int AmountOfTeachingHours { get; set; }
        public int TimeAllocationId { get; set; }
        [DisplayName("Remaining Time")]
        public int RemainingTime { get; set; }
        public string url { get; set; }
    }

    public class EditCalendarHelper
    {
        public List<TimeAllocationHelper> DateHelper { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int ProgramTeachingHoursPerDay { get; set; }
        public List<bool> holidays { get; set; }

        public int month { get; set; }
        public int year { get; set; }
        public int ProgramId { get; set; }

    }
}