namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class programAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgramDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramName = c.String(),
                        ProgramStartDate = c.DateTime(nullable: false),
                        ProgramEndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Courses", "ProgramId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "ProgramId");
            DropTable("dbo.ProgramDetails");
        }
    }
}
