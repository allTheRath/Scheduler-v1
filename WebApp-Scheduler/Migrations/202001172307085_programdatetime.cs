namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class programdatetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProgramDetails", "ProgramStartDate", c => c.DateTime());
            AlterColumn("dbo.ProgramDetails", "ProgramEndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProgramDetails", "ProgramEndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProgramDetails", "ProgramStartDate", c => c.DateTime(nullable: false));
        }
    }
}
