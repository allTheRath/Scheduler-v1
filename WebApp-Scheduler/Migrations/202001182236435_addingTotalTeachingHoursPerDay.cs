namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingTotalTeachingHoursPerDay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramDetails", "TotalTeachingHoursOfDay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramDetails", "TotalTeachingHoursOfDay");
        }
    }
}
