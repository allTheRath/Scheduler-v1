namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "ScheduleTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Courses", "ScheduleTypeId");
            AddForeignKey("dbo.Courses", "ScheduleTypeId", "dbo.ScheduleTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "ScheduleTypeId", "dbo.ScheduleTypes");
            DropIndex("dbo.Courses", new[] { "ScheduleTypeId" });
            DropColumn("dbo.Courses", "ScheduleTypeId");
        }
    }
}
