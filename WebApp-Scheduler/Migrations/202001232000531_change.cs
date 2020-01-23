namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramDetails", "StartTime", c => c.DateTime());
            AddColumn("dbo.ProgramDetails", "EndTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramDetails", "EndTime");
            DropColumn("dbo.ProgramDetails", "StartTime");
        }
    }
}
