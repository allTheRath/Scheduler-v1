namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newContextChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseWithTimeAllocations", "ProgramId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseWithTimeAllocations", "ProgramId");
        }
    }
}
