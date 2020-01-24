namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designChangeForTimeline : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeAllocationHelpers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        RemainingTime = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TimeAllocationHelpers");
        }
    }
}
