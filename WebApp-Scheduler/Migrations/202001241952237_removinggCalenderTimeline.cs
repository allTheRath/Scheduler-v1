namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removinggCalenderTimeline : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CalendarCoursePerDays", "CalendarId", "dbo.Calendars");
            DropIndex("dbo.CalendarCoursePerDays", new[] { "CalendarId" });
            DropTable("dbo.CalendarCoursePerDays");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CalendarCoursePerDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CalendarId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        startTimeForDay = c.String(),
                        endTimeForDay = c.String(),
                        TeachingHours = c.Int(nullable: false),
                        Topic = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.CalendarCoursePerDays", "CalendarId");
            AddForeignKey("dbo.CalendarCoursePerDays", "CalendarId", "dbo.Calendars", "Id", cascadeDelete: true);
        }
    }
}
