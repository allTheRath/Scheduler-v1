namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingChangestocourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Calendars", "ProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.Calendars", "IsChanged", c => c.Boolean(nullable: false));
            AddColumn("dbo.Courses", "CourseCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "CourseCode");
            DropColumn("dbo.Calendars", "IsChanged");
            DropColumn("dbo.Calendars", "ProgramId");
        }
    }
}
