namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class calendarAbility : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Calendars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsHoliday = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calendars", t => t.CalendarId, cascadeDelete: true)
                .Index(t => t.CalendarId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CalendarCoursePerDays", "CalendarId", "dbo.Calendars");
            DropIndex("dbo.CalendarCoursePerDays", new[] { "CalendarId" });
            DropTable("dbo.CalendarCoursePerDays");
            DropTable("dbo.Calendars");
        }
    }
}
