using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApp_Scheduler.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Instructor { get; set; }
        public int ContactHours { get; set; }
        public int HoursPerDay { get; set; }
        public int? NumberOfDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int ScheduleTypeId { get; set; }
        public virtual ScheduleType ScheduleType { get; set; }
    }

    public class PreRequisite
    {
        public int Id { get; set; }
        public int RequiredCourseId { get; set; }
        public int ActualCourseId { get; set; }
    }

    public class ScheduleType
    {

        public int Id { get; set; }
        public string DayOption { get; set; }
        public virtual ICollection<Course> CoursesWithScheduleType { get; set; }

    }

    public class ScheduleContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<PreRequisite> PrerequisiteCourses { get; set; }
        public DbSet<ScheduleType> TeachingDays { get; set; }
    }
}