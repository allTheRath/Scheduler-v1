namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedCourseHelperDataContextOfData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduleHelperContextDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        courseId = c.Int(nullable: false),
                        CourseName = c.String(),
                        TeachingDays = c.String(),
                        TotalHoursPerDay = c.Int(nullable: false),
                        OverallTotalHours = c.Int(nullable: false),
                        NoOfTeachingDays = c.Int(nullable: false),
                        ProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ScheduleHelperContextDatas");
        }
    }
}
