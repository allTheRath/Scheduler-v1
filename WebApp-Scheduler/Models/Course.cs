using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApp_Scheduler.Models
{
    public class ProgramDetails
    {
        public int Id { get; set; }
        [DisplayName("Program Name")]
        public string ProgramName { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayName("Program Start Date")]
        public DateTime? ProgramStartDate { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayName("Program End Date")]
        public DateTime? ProgramEndDate { get; set; }
    
        [DisplayName("Hours Of Teaching Per Day")]
        public int TotalTeachingHoursOfDay { get; set; }
        public List<TimeAllocationHelper> GetAllDayInstances(ProgramDetails program)
        {
            char[] days = new char[] { 'M', 'T', 'W', 'R', 'F' };
            // getting program time length
            List<TimeAllocationHelper> daysOfStudy = new List<TimeAllocationHelper>();

            int totalDaysOfEducation = program.CalculateTotalDaysOfEducation(program);
            DateTime startD = program.ProgramStartDate.Value.Date;
            for (int k = 0; k < totalDaysOfEducation; k++)
            {
                if (startD.DayOfWeek != DayOfWeek.Saturday && startD.DayOfWeek != DayOfWeek.Sunday)
                {
                    TimeAllocationHelper dayInstance = new TimeAllocationHelper();
                    dayInstance.RemainingTime = program.TotalTeachingHoursOfDay;
                    dayInstance.Date = startD;
                    int a = (int)startD.DayOfWeek - 1;
                    dayInstance.Day = days[a];
                    daysOfStudy.Add(dayInstance);
                }

                startD = startD.AddDays(1);

            }

            return daysOfStudy;
        }
        public int CalculateTotalDaysOfEducation(ProgramDetails program)
        {
            int yearStart = program.ProgramStartDate.Value.Year;
            int yearEnd = program.ProgramEndDate.Value.Year;
            int howManyYear = yearEnd - yearStart + 1;
            int startDayInt = program.ProgramStartDate.Value.DayOfYear;
            int endDayInt = program.ProgramEndDate.Value.DayOfYear;
            int TotalDaysOfEducation = (howManyYear * 365) - startDayInt + endDayInt;
            return TotalDaysOfEducation;
        }
    }

    public class Course
    {
        public int Id { get; set; }
        [DisplayName("Course")]
        public string CourseName { get; set; }
        public string Instructor { get; set; }
        public int ContactHours { get; set; }
        public int HoursPerDay { get; set; }
        [DisplayName("# Of Days")]
        public int? NumberOfDays { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        [DisplayName("Schedule Type(MWF/TTH etc)")]
        public int ScheduleTypeId { get; set; }
        public virtual ScheduleType ScheduleType { get; set; }
        
        public int ProgramId { get; set; }
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
        public DbSet<ProgramDetails> Programs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        
    }
    public class CourseSelectionViewModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }

    }

    public class ListOfCourseSelectionViewModel
    {
        public int CourseId { get; set; }
        public List<CourseSelectionViewModel> selectedCourses { get; set; }
        public List<CourseSelectionViewModel> selectOptions { get; set; }
    }
}