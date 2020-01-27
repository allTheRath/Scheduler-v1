namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedCourseHelperDataContextOf : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.ScheduleHelperContexts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ScheduleHelperContexts",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        CourseName = c.String(),
                        TeachingDays = c.String(),
                        TotalHoursPerDay = c.Int(nullable: false),
                        OverallTotalHours = c.Int(nullable: false),
                        NoOfTeachingDays = c.Int(nullable: false),
                        ProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseId);
            
        }
    }
}
