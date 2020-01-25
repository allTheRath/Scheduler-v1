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
        
    }
    public class DateHelper
    {
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int ProgramId { get; set; }
    }

    public class SelectHolidayHelper
    {
        public List<Calendar> CalendarForMonth { get; set; }
        
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }

}