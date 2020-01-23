﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebApp_Scheduler.Models;

namespace WebApp_Scheduler.Controllers
{
    public class SchedulesController : Controller
    {
        private ScheduleContext db = new ScheduleContext();

        // GET: Schedules
        public ActionResult Index(int? programId)
        {
            if (programId == null)
            {
                throw new Exception("Program id was not passed. can't generate excel without program details.");
            }
            int proId = (int)programId;
            Table table = new Table();
            List<string> columns = Table.GetColumNames();
            table.ColumNames = columns;

            List<string> data = table.GetDataFromDatabaseCourses(db, proId);
            table.Data = data;
            table.Program = db.Programs.Find(programId);
            bool saved = table.WriteTableToFile(table);
            if (saved == true)
            {
                return RedirectToAction("Index", "Courses", new { IdOfProgram = proId });
            }
            return View();
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
