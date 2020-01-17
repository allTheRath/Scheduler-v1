namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseName = c.String(),
                        Instructor = c.String(),
                        ContactHours = c.Int(nullable: false),
                        HoursPerDay = c.Int(nullable: false),
                        NumberOfDays = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PreRequisites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequiredCourseId = c.Int(nullable: false),
                        ActualCourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScheduleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DayOption = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ScheduleTypes");
            DropTable("dbo.PreRequisites");
            DropTable("dbo.Courses");
        }
    }
}
