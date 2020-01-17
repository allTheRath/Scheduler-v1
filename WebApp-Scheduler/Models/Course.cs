﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApp_Scheduler.Models
{
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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [DisplayName("Schedule Type(MWF/TTH etc)")]
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