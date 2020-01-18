using Grpc.Core;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Helpers;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;


namespace WebApp_Scheduler.Models
{
    public class Schedule
    {
        public int Id { get; set; }

    }

    public class OptionViewModel
    {
        public List<SingleOption> Options { get; set; }
    }

    public class SingleOption
    {
        public List<CourseDataHelperViewModel> Option { get; set; }
    }

    public class CourseDataHelperViewModel
    {
        public int CourseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Table
    {
        public List<string> ColumNames { get; set; }
        public List<string> Data { get; set; }

        public static List<string> GetColumNames()
        {
            var columnNames = new string[] { "Course", "Prerequisites", "Instructor", "ContactHours", "HoursPerDay", "# Of Days", "Schedule Type(MWF/TTH) etc", "Start Date", "End Date" };
     
            return columnNames.ToList();
        }

        public List<string> GetDataFromDatabaseCourses(ScheduleContext db)
        {
            List<string> data = new List<string>();
            var courses = db.Courses.ToList();
            foreach(var c in courses)
            {
                data.Add(c.CourseName);
                var courseIDs = db.PrerequisiteCourses.Where(x => x.ActualCourseId == c.Id).ToList().Select(x => x.RequiredCourseId).ToList();
                string prerequsites = "";
                for(int i = 0; i < courseIDs.Count(); i++)
                {
                    var courseRetrived = db.Courses.Find(courseIDs[i]);
                    prerequsites += courseRetrived.CourseName + " , ";
                }
                data.Add(prerequsites);
                data.Add(c.Instructor);
                data.Add(c.ContactHours.ToString());
                data.Add(c.HoursPerDay.ToString());
                data.Add(c.NumberOfDays.ToString());
                var schedule = db.TeachingDays.Find(c.ScheduleTypeId);
                data.Add(schedule.DayOption);
                data.Add(Convert.ToString(c.StartDate));
                data.Add(Convert.ToString(c.EndDate));
                
            }

            return data;
        }

        public bool WriteTableToFile(Table table)
        {
        
            Application xlApp = new Application();
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            for (int i = 0; i < table.ColumNames.Count(); i++)
            {
                xlWorkSheet.Cells[1, i + 1] = table.ColumNames[i];
            }
            if(table.Data != null)
            {
                int rowLength = table.ColumNames.Count();
                int howmanyRows = table.Data.Count() / rowLength;
                int counter = 0;
                for (int j = 2; j <= howmanyRows; j++)
                {
                    for (int k = 1; k <= rowLength; k++)
                    {
                        xlWorkSheet.Cells[j, k] = table.Data[counter];
                        counter++;
                    }
                }
            }
            
            //adding header column..
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Excel/"), "Option.xls");

            xlWorkBook.SaveAs(path, XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            return true;
        }
    }


}